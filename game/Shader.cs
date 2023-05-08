namespace Game;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader {
    private readonly int _handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public Shader(string vertPath, string fragPath) {
        // Load vertex shader and compile
        var shaderSource = File.ReadAllText(vertPath);
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, shaderSource);
        CompileShader(vertexShader);

        // Load fragment shader and compile
        shaderSource = File.ReadAllText(fragPath);
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, shaderSource);
        CompileShader(fragmentShader);

        // Create and link program
        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);
        LinkProgram(_handle);

        // Detach shaders, and then delete them.
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        // Cache all the shader uniform locations
        GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        _uniformLocations = new Dictionary<string, int>();
        for (var i = 0; i < numberOfUniforms; i++) {
            var key = GL.GetActiveUniform(_handle, i, out _, out _);
            var location = GL.GetUniformLocation(_handle, key);
            _uniformLocations.Add(key, location);
        }
    }

    private static void CompileShader(int shader) {
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code == (int)All.True) return;

        var infoLog = GL.GetShaderInfoLog(shader);
        throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
    }

    private static void LinkProgram(int program) {
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code == (int)All.True) return;

        throw new Exception($"Error occurred whilst linking Program({program})");
    }

    public void Use() {
        GL.UseProgram(_handle);
    }

    public int GetAttribLocation(string attribName) {
        return GL.GetAttribLocation(_handle, attribName);
    }

    public void SetInt(string name, int data) {
        if (!_uniformLocations.ContainsKey(name)) return;
        GL.Uniform1(_uniformLocations[name], data);
    }

    public void SetFloat(string name, float data) {
        if (!_uniformLocations.ContainsKey(name)) return;
        GL.Uniform1(_uniformLocations[name], data);
    }

    public void SetMatrix4(string name, Matrix4 data) {
        if (!_uniformLocations.ContainsKey(name)) return;
        GL.UniformMatrix4(_uniformLocations[name], false, ref data);
    }

    public void SetVector3(string name, Vector3 data) {
        if (!_uniformLocations.ContainsKey(name)) return;
        GL.Uniform3(_uniformLocations[name], data);
    }

    public void PresetShaders(Camera camera, Vector3[] pointLightPositions) {
        Use();
        
        SetInt("material.diffuse", 0);
        SetInt("material.specular", 1);
        SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        SetFloat("material.shininess", 32.0f);
        
        SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
        SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
        SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
        SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

        for (var i = 0; i < pointLightPositions.Length; i++) {
            SetVector3($"pointLights[{i}].position", pointLightPositions[i]);
            SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
            SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
            SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
            SetFloat($"pointLights[{i}].constant", 1.0f);
            SetFloat($"pointLights[{i}].linear", 0.09f);
            SetFloat($"pointLights[{i}].quadratic", 0.032f);
        }

        SetVector3("spotLight.position", camera.Position);
        SetVector3("spotLight.direction", camera.Front);
        SetVector3("spotLight.ambient", new Vector3(0.0f, 0.0f, 0.0f));
        SetVector3("spotLight.diffuse", new Vector3(1.0f, 1.0f, 1.0f));
        SetVector3("spotLight.specular", new Vector3(1.0f, 1.0f, 1.0f));
        SetFloat("spotLight.constant", 1.0f);
        SetFloat("spotLight.linear", 0.09f);
        SetFloat("spotLight.quadratic", 0.032f);
        SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
        SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
    }
    
    public void Dispose() {
        GL.DeleteProgram(_handle);
    }
}