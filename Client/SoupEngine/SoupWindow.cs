using MclTK;
using OpenTK.Mathematics;
using SoupEngine.Objects;

namespace SoupEngine;

public class SoupWindow : MclWindow
{
    protected List<GravitySource> GravitySources = new List<GravitySource>();

    protected override void OnUpdateFrame(double dt)
    {
        for (int i = 0; i < MclObjects.Count; i++) UpdateObject(MclObjects[i]);
    }
    
    private void UpdateObject(MclObject obj)
    {
        if (obj is SoupObject)
        {
            SoupObject physObj = (SoupObject)obj;
            Vector3 pos = physObj.Position;

            for (int i = 0; i < GravitySources.Count; i++)
            {
                GravitySource gravSrc = GravitySources[i];
                Vector3 gravSrcPos = gravSrc.Position;
                const float planetRadius = 1.0f;
                if ((gravSrcPos - pos).Length > planetRadius)
                {
                    Vector3 forceGravity = CalculateGravityVector(1.0f, pos, gravSrcPos, 1.0f, gravSrc.Mass);
                    physObj.ApplyForce(forceGravity);
                }
                else physObj.Velocity /= 2.0f;
            }
            
            physObj.Update();
        }
    }

    private static Vector3 CalculateGravityVector(float G, Vector3 p0, Vector3 p1, float m0, float m1)
    {
        Vector3 dif = p1 - p0;
        float r = dif.Length;
        float strength = NewtonianGravity(G, m0, m1, r);
        Vector3 direction = (r == 0) ? Vector3.Zero : dif.Normalized();
        return direction * strength;
    }

    // https://en.wikipedia.org/wiki/Newton's_law_of_universal_gravitation
    private static float NewtonianGravity(float G, float m0, float m1, float r)
    {
        return G * ((m0 * m1) / (r * r));
    }
}