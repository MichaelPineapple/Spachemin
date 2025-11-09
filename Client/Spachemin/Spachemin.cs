using SpacheNet;
using SpachEngine;

namespace Spachemin;

public class Spachemin : SpachWindow
{
    public Spachemin(SpacheNetClient net) : base(net)
    { 
        Size = (700, 700);
        Title = "Spachemin";
    }

    protected override void OnLoadGraphics()
    {
        string pathApp = Path.GetDirectoryName(Environment.ProcessPath) + "/Assets/";
        string pathShaders = pathApp + "Shaders/";
        string pathMeshes = pathApp + "Meshes/";
        string pathTextures = pathApp + "Textures/";
        
        Shader shader = new Shader(pathShaders + "Default/default.vert", pathShaders + "Default/default.frag");
        Mesh meshPlayer = new Mesh(pathMeshes + "cube.mesh", shader);
        Mesh meshGround = new Mesh(pathMeshes + "ground.mesh", shader);
        Texture texPlayer = new Texture(pathTextures + "grid.png");
        Texture texGround = new Texture(pathTextures + "grid.png");
        
        SetDefaultShader(shader);
        SetPlayerMesh(meshPlayer, texPlayer);
        SetGroundMesh(meshGround, texGround);
        
        base.OnLoadGraphics();
    }
}

