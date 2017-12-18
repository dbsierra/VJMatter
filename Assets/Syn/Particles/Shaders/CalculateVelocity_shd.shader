Shader "Unlit/CalculateVelocity_shd"
{

Properties
{
    _MainTex("Texture", 2D) = "white" {}
    _Mode("Mode", Range(0,1)) = 0
}
SubShader
{
    Tags{ "RenderType" = "Opaque" }
    LOD 100

    Pass
    {
        ZTest Always
        ZWrite Off

        CGPROGRAM
        #pragma vertex defaultVert
        #pragma fragment frag
        #pragma target 5.0

        #include "UnityCG.cginc"
        #include "../../BlitIncludes.cginc"
        #include "../../Noise/noiseSimplex.cginc"

        // Parameters
        float _DragCoeff;
        float _PulserMult;
        float _AntiPulserMult;
        float _AccelMultiplier;
        float _VelocityLimit;
        float _VelocityMult;
        float _TargetForceMult;
        float _LissForceMult;
        float _LissRadius;
        float _LissNoiseAmp;
        float _LissNoiseFreq;
        float _ShapePercentage;
        float4 _LissFrequencies;
        float3 _PulserTarget;
        float4 _Target;

        int _Shape;

        // Textures
        sampler2D _Position;
        sampler2D _InitialPos;
        sampler2D _Random;
        sampler2D _NoveltyHistory;

        float3 RotateAroundAxis(float3 vec, float3 axis, float angle)
        {
            float s, c;
            sincos(angle, s, c);
            float t = 1 - c;

            float3x3 rotationMatrix =
            {
                (t * axis.x * axis.x) + c,
                (t * axis.x * axis.y) - (axis.z * s),
                (t * axis.x * axis.z) + (axis.y * s),

                (t * axis.y * axis.x) + (axis.z * s),
                (t * axis.y * axis.y) + c,
                (t * axis.y * axis.z) - (axis.x * s),

                (t * axis.z * axis.x) - (axis.y * s),
                (t * axis.z * axis.y) + (axis.x * s),
                (t * axis.z * axis.z) + c
            };

            return mul(vec, rotationMatrix);
        }

        float rand(float2 co) {
            return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
        }

        // i = normalized particle index (index from 0 - 1 for each particle)
        float3 Spiral(float i, float offset)
        {
            float3 nextPos = float3(0, 0, 0);
            nextPos.x = sin(i*6.28*10.) * i;
            nextPos.y = cos(i*6.28*10.) * i;
            nextPos.z = i * (sin(i * 500. - offset)*.5 + sin(i * 500. + offset)*.5);
            return nextPos;
        }

        float3 Sphere(float4 ppos, float3 origin, float radius, float4 initialPos )
        {
            float f = 35.;
            float i = ppos.w;
            float th = i * 6.28;
            float ph = i * 3.14;
            float3 nextPos = float3(0, 0, 0);
            nextPos.y = cos(f*th) * sin(ph) * radius + origin.y;
            nextPos.z = sin(f*th) * sin(ph) * radius + origin.z;
            nextPos.x = cos(ph) * radius + origin.x;

            float3 Normal = nextPos;
            float3 seed = nextPos;
            seed.z += _Time.y;
            nextPos += Normal * snoise(5* seed) * .2;

            return nextPos;
        }

        float3 Lissajous(float i, float3 frequencies, float radius)
        {
            float3 nextPos = float3(0, 0, 0);

            nextPos.x = sin(i*6.28 * frequencies.x) * radius;
            nextPos.y = cos(i*6.28 * frequencies.y + _Time.y*2.5) * radius;
            nextPos.z = sin(i*6.28 * frequencies.z + _Time.y) * radius;

            // Add noise to Lissajou pattern
            float3 Normal = nextPos;
            float3 seed = nextPos;
            seed.z += _Time.y;
            nextPos += Normal * snoise(_LissNoiseFreq * seed) * _LissNoiseAmp;

            return nextPos;
        }
        
        float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + s - a1*b2 - b1 / a2 - a1;
        }	

        // Force is linear up to 1 unit distance, otherwise it tapers off as distance increases
        float ForceMagnitudeFromDistance(float distance)
        {
            float force = (distance < 1.f) ? (2-distance) : (1.f / distance);
            return force;// *-0.005f;
        }

        float3 ForceFromTarget(float3 ppos, float3 target)
        {
            // Vector pointing towards target
            float3 vDelta = ppos - target;

            // Distance from target
            float distance = length(vDelta);

            // 0 - 1, magnitude is 2 - 1, further away, magnitude scales down with distance
            float forceMagnitude = -1*ForceMagnitudeFromDistance(distance);

            // forceMagnitude = forceMagnitude / max(distance, .01) );

            // Force vector
            float3 vForce = normalize(vDelta) * forceMagnitude;

            return vForce;

            //  return normalize(-1 * vDelta * ForceMagnitudeFromDistance(distance));

            //  return normalize( RotateAroundAxis(vForce, normalize(ppos), 3.14159 / 24.) );
        }

        float3 ForceFromPulser(float3 ppos, float3 pulserPos, float pulserMultiplier)
        {
            float3 vForce = float3(0, 0, 0);

            // Vector pointing towards pulser center 
            float3 vDelta = ppos - pulserPos;

            // Distance from pulser center
            float distance = length(vDelta);

            // If particle is within 1 unit of pulser center, distance is mapped as 1-2, with 2 being at center 
            // and 1 being at 1 unit from center. If particle is not within 1 unit of center, return 1/distance,
            // so at far distances this value is very small
            float distanceScalar = ForceMagnitudeFromDistance(distance);

            // This is the current RMS deviation from the average, from -1 to 1
            float pulseMagnitude = tex2D(_NoveltyHistory, float2(.5, saturate(distance))).b * pulserMultiplier;

            // Accentuate the negative forces more
            //  if (pulseMagnitude < 0)
            //      pulseMagnitude *= 1.1;

            pulseMagnitude *= distanceScalar;// saturate(2 - distance); TOGGLE: This gives a different look
            
            // Always gravitate towards pulser center by some amount
            float attractionMagnitude = -1 * _AntiPulserMult;

            // vForce = normalize(vDelta) * (forceMagnitude / max(distance,.01) );
            
            vForce = normalize(vDelta) * pulseMagnitude + normalize(vDelta) * attractionMagnitude;
            // return RotateAroundAxis(vForce, normalize(ppos), 3.14159 / 24.);

            // Add spin
            // return normalize(ppos) * -1*(-1 * rmsHistory[0] * 100.);
            return vForce;
        }

        float3 Impulse(float3 ppos, float3 impulsePos, float magnitude)
        {
            float3 vForce = float3(0, 0, 0);

            float distance = length(ppos - impulsePos);

            //  vForce = magnitude * normalize(ppos) * saturate(.1-distance);

            return vForce;
        }

        float3 PerlinForceField(float4 ppos) {
            /* A time-warying Simplex force field */
            return
                normalize( float3(
                    snoise(float3(ppos.x, ppos.y, ppos.z + _Time.y)),
                    snoise(float3(ppos.x + 400. + _Time.y, ppos.y, ppos.z)),
                    snoise(float3(ppos.x + 500., ppos.y + _Time.y, ppos.z))
                    ) );
        }

        fixed4 frag(v2f i) : SV_Target
        {

            float4 vel = tex2D(_MainTex, i.uv);
            float4 pos = tex2D(_Position, i.uv);
            float4 random = tex2D(_Random, i.uv);
            float3 acceleration = float3(0, 0, 0);

            float speed = length(vel);

            float dragCoef = _DragCoeff * Map(random.r,0,1,.9,1);
            float3 drag = dragCoef * (speed * speed) * (-1 * vel.xyz);

            float3 targetPoint = _Target.xyz;
            float3 pulserPos = _PulserTarget.xyz;
            float3 lissFrequencies = _LissFrequencies.xyz;

            // Pulser force
            float3 PulserForce = ForceFromPulser(pos.xyz, pulserPos, _PulserMult);

            // Interaction point forces
            float3 InteractionForce = ForceFromTarget(pos.xyz, targetPoint) * _Target.w * _TargetForceMult;

            // Lissajou force
            float3 LissajousForce = ForceFromTarget(pos.xyz, Lissajous(pos.w, lissFrequencies, _LissRadius)) * _LissForceMult;

            acceleration += PulserForce * (1-_ShapePercentage);
            acceleration += InteractionForce * (1-_ShapePercentage);
            acceleration += LissajousForce * (1-_ShapePercentage);

            acceleration += ForceFromTarget(pos.xyz, 1.9f*float3(i.uv.x-.5f, i.uv.y-.5f, 0.0f) ) * _ShapePercentage * 10.0f;

            acceleration += drag;

            vel.xyz += acceleration * _AccelMultiplier * unity_DeltaTime.x;

           // vel.xyz = ForceFromTarget(pos.xyz, 2*float3(i.uv.x - .5, i.uv.y - .5, 0) );

          
            vel.xyz *= _VelocityMult ;

        //    if (_Shape != 0)
        //        vel.xyz = float3(0, 0, 0);
            return vel;
           // return clamp(vel, -1*_VelocityLimit, _VelocityLimit);
        }
        ENDCG
    }
}
}

