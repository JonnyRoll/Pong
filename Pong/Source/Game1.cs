using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private const float movmentSpeed = 400f;// pixels per secound (so all computers move at the same speed) 
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _batTexture; // holds the bat image
    private Texture2D _ballTexture;

    // are the variables that will hold the bats. 
    private Vector2 _leftBat; 
    private Vector2 _rightBat;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here starting values

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // TODO: use this.Content to load your game content here
        _batTexture = Content.Load<Texture2D>("bat");
        _ballTexture = Content.Load<Texture2D>("ball");
        
        // setting the starting states of both bats in load content...
        float middleOfScreenHeight =  (float) GraphicsDevice.Viewport.Height / 2;
        float endOfScreenWidth = (float) GraphicsDevice.Viewport.Width ;

        _leftBat = new Vector2(30, middleOfScreenHeight);
        _rightBat = new Vector2(endOfScreenWidth - 30 - _batTexture.Width,  middleOfScreenHeight);
    }
    
    // core of game logic, gets called many tikmes
    protected override void Update(GameTime gameTime)
    {
#if !__IOS__
        // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
        // (iOS does not allow programmatically closing an app.)
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
#endif

        
        KeyboardState key = Keyboard.GetState(); // starts keyboard input here
        float timePassed = (float)gameTime.ElapsedGameTime.TotalSeconds; // totoal time passed 
        Rectangle screen = GraphicsDevice.Viewport.Bounds; // the game window 

        _leftBat.Y +=  VerticalGetDirection(key, Keys.W, Keys.S) * timePassed *  movmentSpeed; // so that we account for slow or fast machines 
        _leftBat.X += HorrizontalGetDirection(key, Keys.A, Keys.D) * timePassed *  movmentSpeed;
        _rightBat.Y +=  VerticalGetDirection(key, Keys.Up, Keys.Down) * timePassed *  movmentSpeed; // so that we account for slow or fast machines 
        _rightBat.X += HorrizontalGetDirection(key, Keys.Left, Keys.Right) * timePassed *  movmentSpeed;
        // now to make sure we don't leave the screen!
        _leftBat.Y = MathHelper.Clamp(_leftBat.Y, 0, screen.Height - _batTexture.Height);
        _rightBat.Y = MathHelper.Clamp(_rightBat.Y, 0, screen.Height - _batTexture.Height);
        _leftBat.X = MathHelper.Clamp(_leftBat.X, 0, screen.Width/2f - _batTexture.Width);
        _rightBat.X = MathHelper.Clamp(_rightBat.X, screen.Width/2f, screen.Width - _batTexture.Width);
        
        base.Update(gameTime);
    }

    // this gets called after every call of update
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        int middleOfWindow = GraphicsDevice.Viewport.Width / 2;
        int heightOfWindow = GraphicsDevice.Viewport.Height;
        int currHeight = 10;
        _spriteBatch.Begin(); // starts a new set of draw calls
        while (currHeight <= heightOfWindow - 10)
        {
            _spriteBatch.Draw(_ballTexture, new Rectangle(middleOfWindow - 4, currHeight, 8, 8), Color.White * 0.2f); // the image we use doesn't matter at all, since we say the shape is a rectangle
            currHeight += 20;
        }
        
        // Drawing the bats
        _spriteBatch.Draw(_batTexture, _leftBat, Color.White);
        _spriteBatch.Draw(_batTexture, _rightBat, Color.White);
        _spriteBatch.End(); // ends the set of draw calls
        base.Draw(gameTime);
    }
    
    /// <summary>
    /// Moves the bat the appropriate amount
    /// </summary>
    /// <param name="keyboard"></param>
    /// <param name="up"></param>
    /// <param name="down"></param>
    /// <returns></returns>
    private static float VerticalGetDirection(KeyboardState keyboard, Keys up, Keys down)
    {
        if (keyboard.IsKeyDown(up))
        {
            return -1;
        }

        else if (keyboard.IsKeyDown(down))
        {
            return 1;
        }

        return 0;
    }
    
    private static float HorrizontalGetDirection(KeyboardState keyboard, Keys left, Keys right)
    {
        if (keyboard.IsKeyDown(left))
        {
            return -1;
        }

        else if (keyboard.IsKeyDown(right))
        {
            return 1;
        }

        return 0;
    }
}
