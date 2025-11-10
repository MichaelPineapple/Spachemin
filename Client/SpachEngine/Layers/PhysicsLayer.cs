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
                Vector3 toSrc = gravSrc.position - pos;
                float gravStrength = gravSrc.mass / (toSrc.Length * toSrc.Length);
                Vector3 gravDirection = (toSrc.Length == 0) ? Vector3.Zero : toSrc.Normalized();
                Vector3 gravity = gravDirection * gravStrength;
            
                const float planetRadius = 1.0f;
                if (toSrc.Length > planetRadius) physObj.ApplyForce(gravity);
                else physObj.velocity /= 2.0f;
            }
            
            physObj.Update();
        }
    }
}