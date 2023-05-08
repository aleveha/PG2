using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Game;

struct Vertex {
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;
}

public class Mesh {
    private readonly Shader _shader;
    private readonly Texture? _texture;

    private readonly List<Vertex> _vertices;
    
    private readonly int _vao, _vbo;

    private Mesh(Shader shader, Texture? texture) {
        _shader = shader;
        _texture = texture;

        // Generate the vertex array object, vertex buffer object and element buffer object
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        _vertices = new List<Vertex>();
    }

    public Mesh(Shader shader, float[] vertices, Texture? texture = null) : this(shader, texture) {
        // Map the vertices, uvs and normals to the vertex struct
        for (var i = 0; i < vertices.Length; i += 8) {
            _vertices.Add(
                new Vertex {
                    Position = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]),
                    Normal = new Vector3(vertices[i + 3], vertices[i + 4], vertices[i + 5]),
                    TexCoords = new Vector2(vertices[i + 6], vertices[i + 7])
                }
            );
        }
        
        InitShader();
    }

    public Mesh(Shader shader, string objectFilePath, Texture? texture = null) : this(shader, texture) {
        // Read the vertices, uvs, normals and indices from the obj file
        var (vertices, uvs, normals) = ParseObjFile(objectFilePath);

        // Map the vertices, uvs and normals to the vertex struct
        for (var i = 0; i < vertices.Count; i++) {
            _vertices.Add(new Vertex {
                Position = vertices[i],
                Normal = normals[i],
                TexCoords = uvs[i]
            });
        }
        
        InitShader();
    }

    private void InitShader() {
        var vertexSize = Marshal.SizeOf<Vertex>();

        // Bind the vertex array object and set the vertex attributes
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * vertexSize, _vertices.ToArray(),
            BufferUsageHint.StaticDraw);

        // Set the vertex attributes
        var positionLocation = _shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, vertexSize,
            Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)));

        var normalLocation = _shader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, vertexSize,
            Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));

        var texCoordLocation = _shader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, vertexSize,
            Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoords)));

        // Unbind the vertex array object, vertex buffer object and element buffer object
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Draw(Camera camera, Matrix4? model = null) {
        _shader.Use();

        // Error while binding texture: UNSUPPORTED (log once): POSSIBLE ISSUE: unit 1 GLD_TEXTURE_INDEX_2D is unloadable and bound to sampler type (Float) - using zero texture because texture unloadable
        _texture?.Use(TextureUnit.Texture0);

        _shader.SetMatrix4("model", model ?? Matrix4.Identity);
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetVector3("viewPosition", camera.Position);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
        GL.BindVertexArray(0);
    }
    
    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    private static Tuple<List<Vector3>, List<Vector2>, List<Vector3>> ParseObjFile(string path) {
        var vertexIndices = new List<uint>();
        var uvIndices = new List<uint>();
        var normalIndices = new List<uint>();
        var tempVertices = new List<Vector3>();
        var tempUVs = new List<Vector2>();
        var tempNormals = new List<Vector3>();

        using (var file = new StreamReader(path)) {
            while (!file.EndOfStream) {
                var line = file.ReadLine();
                if (line == null) continue;

                var lineParts = line.Replace(".", ",").Split(' ');
                var lineHeader = lineParts[0];

                switch (lineHeader) {
                    case "v":
                        var vertex = new Vector3(float.Parse(lineParts[1]), float.Parse(lineParts[2]),
                            float.Parse(lineParts[3]));
                        tempVertices.Add(vertex);
                        break;
                    case "vt":
                        var uv = new Vector2(float.Parse(lineParts[2]), float.Parse(lineParts[1]));
                        tempUVs.Add(uv);
                        break;
                    case "vn":
                        var normal = new Vector3(float.Parse(lineParts[1]), float.Parse(lineParts[2]),
                            float.Parse(lineParts[3]));
                        tempNormals.Add(normal);
                        break;
                    case "f":
                        for (var i = 1; i <= 3; i++) {
                            var vertexParts = lineParts[i].Split('/');
                            var vertexIndex = uint.Parse(vertexParts[0]);
                            var uvIndex = uint.Parse(vertexParts[1]);
                            var normalIndex = uint.Parse(vertexParts[2]);

                            vertexIndices.Add(vertexIndex);
                            uvIndices.Add(uvIndex);
                            normalIndices.Add(normalIndex);
                        }

                        break;
                }
            }
        }

        var outVertices = vertexIndices.Select(vertexIndex => tempVertices[(int)vertexIndex - 1]).ToList();
        var outUVs = uvIndices.Select(uvIndex => tempUVs[(int)uvIndex - 1]).ToList();
        var outNormals = normalIndices.Select(normalIndex => tempNormals[(int)normalIndex - 1]).ToList();

        return new Tuple<List<Vector3>, List<Vector2>, List<Vector3>>(outVertices, outUVs, outNormals);
    }
}