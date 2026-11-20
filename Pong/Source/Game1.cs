using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private const float MovmentSpeed = 400f;// pixels per secound (so all computers move at the same speed) 
    private const float BallSpeed = 300f;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _batTexture; // holds the bat image
    private Texture2D _ballTexture;

    // are the variables that will hold the bats. 
    private Vector2 _leftBat; 
    private Vector2 _rightBat;
    
    // for the ball
    private Vector2 _ballPosition;
    private Vector2 _ballVelocity;
    
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
        
        // server the ball 
        ResetBall(new Random().Next(2) == 0 ? -1 : 1); // randomly choses which side to serve.
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

        _leftBat.Y +=  VerticalGetDirection(key, Keys.W, Keys.S) * timePassed *  MovmentSpeed; // so that we account for slow or fast machines 
        _leftBat.X += Colligion.HorrizontalGetDirection(key, Keys.A, Keys.D) * timePassed *  MovmentSpeed;
        _rightBat.Y +=  VerticalGetDirection(key, Keys.Up, Keys.Down) * timePassed *  MovmentSpeed; // so that we account for slow or fast machines 
        _rightBat.X += Colligion.HorrizontalGetDirection(key, Keys.Left, Keys.Right) * timePassed *  MovmentSpeed;
        // now to make sure we don't leave the screen!
        _leftBat.Y = MathHelper.Clamp(_leftBat.Y, 0, screen.Height - _batTexture.Height);
        _rightBat.Y = MathHelper.Clamp(_rightBat.Y, 0, screen.Height - _batTexture.Height);
        _leftBat.X = MathHelper.Clamp(_leftBat.X, 0, screen.Width/2f - _batTexture.Width);
        _rightBat.X = MathHelper.Clamp(_rightBat.X, screen.Width/2f, screen.Width - _batTexture.Width);

        _ballPosition += timePassed * _ballVelocity; // adds the distance the ball moved 
        
        // Bounce the ball off the top and bottom edges.
        if ((_ballPosition.Y < 0 && _ballVelocity.Y < 0) ||
            (_ballPosition.Y > screen.Height - _ballTexture.Height && _ballVelocity.Y > 0))
        {
            _ballVelocity.Y = -_ballVelocity.Y;
        }
        
        // Bounce the ball off a bat.
        if (_ballVelocity.X < 0 && GetBounds(_ballPosition, _ballTexture).Intersects(GetBounds(_leftBat, _batTexture)))
        {
            HitBall(_leftBat);
        }
        else if (_ballVelocity.X > 0 && GetBounds(_ballPosition, _ballTexture).Intersects(GetBounds(_rightBat, _batTexture)))
        {
            HitBall(_rightBat);
        }
        
        // Serve again when the ball goes off the left or right edge.
        if (_ballPosition.X < 0)
        {
            ResetBall(1);
        }
        else if (_ballPosition.X > screen.Width)
        {
            ResetBall(-1);
        }
        
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
        _spriteBatch.Draw(_ballTexture, _ballPosition, Color.White);
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
    
    // ball rests method 
    // direction = 1 (move to the right)
    // direction = -1 (move to the left)
    private void ResetBall(int direction)
    {
        Rectangle screen = GraphicsDevice.Viewport.Bounds;
        _ballPosition = new Vector2(screen.Width - _ballTexture.Width, screen.Height - _ballTexture.Height) / 2f;

        // Start with a random vertical speed, so that each serve is different.
        float verticalSpeed = (Random.Shared.NextSingle() - 0.5f) * BallSpeed;
        _ballVelocity = new Vector2(direction * BallSpeed, verticalSpeed);
    }
    
    // Returns the rectangle that a texture covers at a position.
    private static Rectangle GetBounds(Vector2 position, Texture2D texture)
    {
        return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
    }
    
    private void HitBall(Vector2 bat)
    {
        // Send the ball back 5% faster.
        _ballVelocity.X = -_ballVelocity.X * 1.05f;

        // Change the angle. A hit near the end of the bat gives a steeper angle.
        float ballCenter = _ballPosition.Y + _ballTexture.Height / 2f;
        float batCenter = bat.Y + _batTexture.Height / 2f;
        _ballVelocity.Y = (ballCenter - batCenter) * 6f;
    }
    
    
}
