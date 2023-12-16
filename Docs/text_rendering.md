# Rendering text on screen

The engine provides an easy way to render text on the screen by utilizing the `Textbox` class.

## Textbox class

Textbox class can be used to create static text. The resulting object contains a list of renderables, whose can be rendered with a mesh renderer, by adding the meshes to the rendered scene.

### Creating a new Textbox

To create a textbox just provide the name of the used font in the constructor. It will load the fontmap from the `Fonts/{parameter}` folder. Fontmaps can be generated on this website: https://evanw.github.io/font-texture-generator/

#### Setting the content of the textbox

To init a textbox with a text use the `.WithText` method:

```csharp
private Textbox _textbox = new Textbox("Consolas")
    .WithText("Árvíztűrőtükörfúrógép", new Vector2(0, 200), 1.0f);
```

Parameters:
- `text`: The text to show
- `position`: The absolute position on the screen/rendered surface (defaults to (0,0))
- `scale`: You can scale the text by any arbitrary value (defaults to 1.0f)


### Setting the color of the text

You can set the color of the text in HSV by using the

```csharp
WithHue(float hue);
WithSaturation(float saturation);
WithValue(float value);
```
methods.

## Example usage

### New fields:

```csharp
private Textbox _textbox = new Textbox("Consolas")
    .WithText("Árvíztűrőtükörfúrógép", new Vector2(0, 200), 1.0f);
private Matrix4 _textRendererMatrix = Matrix4.Identity;
private readonly IRenderer _textRenderer;
```

### In the constructor:

```csharp
_textRenderer = textRenderer;
_textRenderer.AddToScene(_text.Text);
```

You may or may not need to enable blending before rendering:

```csharp
_textRenderer.BeforeRender = () => GL.Enable(EnableCap.Blend);
_textRenderer.AfterRender = () =>  GL.Disable(EnableCap.Blend);
```

### OnRenderFrame:
```csharp
_worldRenderer.Clear();
_worldRenderer.RenderToTexture(_renderer, _projectionMatrix, viewMatrix, _modelMatrix);
_worldRenderer.RenderToTexture(_textRenderer, _textRendererMatrix, Matrix4.Identity);

_context.SwapBuffers();
```

### OnResize:
```csharp
_textRendererMatrix = Matrix4.CreateOrthographicOffCenter(0, _context.ClientRectangle.Size.X, _context.ClientRectangle.Size.Y, 0, -1, 1);
```

## Dynamic text rendering

It is possible to render dynamically changing text by calling `WithText` on the textbox instance again and re-adding the new meshes to the renderer:

```csharp
_textbox.WithText(text: $"{fps} FPS", scale: 0.4f);
_textRenderer.Clear();
_textRenderer.AddToScene(_textbox.Text);
```

