using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace SpachEngine.Objects;

public class Texture
{
    internal int handle;
    
    public Texture(string path)
    {
        ImageResult image = LoadTexture(path);
        handle = CreateTexture(image);
    }
    
    private static ImageResult LoadTexture(string path)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        FileStream file = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(file, ColorComponents.RedGreenBlueAlpha);
        file.Close();
        file.Dispose();
        return image;
    }

    private static int CreateTexture(ImageResult image)
    {
        int tex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, tex);
        
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, 
            PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return tex;
    }
}