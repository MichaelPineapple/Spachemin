using OpenTK.Mathematics;
using SpachEngine.Objects;

namespace SpachEngine.Layers;

public class PhysicsLayer : GraphicsLayer
{
    protected List<GravitySource> gravitySources = new List<GravitySource>();

    protected override void OnUpdateFrame(double dt)
    {
        for (int i = 0; i < gameObjects.Count; i++) UpdateObject(gameObjects[i]);
    }
    
    private void UpdateObject(GameObject obj)
    {
        if (obj is PhysicsObject)
        {
            PhysicsObject physObj = (PhysicsObject)obj;
            Vector3 pos = physObj.position;

            for (int i = 0; i < gravitySources.Count; i++)
            {
                GravitySource gravSrc = gravitySources[i];
                Vector3 gravSrcPos = gravSrc.position;
                const float planetRadius = 1.0f;
                if ((gravSrcPos - pos).Length > planetRadius)
                {
                    Vector3 forceGravity = CalculateGravityVector(1.0f, pos, gravSrcPos, 1.0f, gravSrc.mass);
                    physObj.ApplyForce(forceGravity);
                }
                else physObj.velocity /= 2.0f;
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