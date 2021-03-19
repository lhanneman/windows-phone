using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Devices.Notification;
using MemorySquared.Resources;

namespace MemorySquared
{
    public partial class Game : PhoneApplicationPage
    {
        DispatcherTimer gameStartTimer;
        DispatcherTimer delayTimer;
        DispatcherTimer userTimer;
        int nGameCountdown;
        Stack<Rectangle> RectsToRemove = new Stack<Rectangle>();
        
                      
        public Game()
        {
            InitializeComponent();

            // IF WE AREN'T DISPLAYING ADS, REMOVE THE AD CANVAS:
            if (!Globals.bDisplayAds) AdRow.Height = new GridLength(0);

            // IF THE GAME IS PAUSED (OR PREVIOUSLY PAUSED), SHOW THE PAUSE GRID.
            // OTHERWISE, RESET FOR A NEW GAME:
            if (Globals.bGameIsPaused)
            {
                DisableRectangles();
                ShowPauseGrid();
            }
            else
            {
                NewGame();
            }
            
            // INITIALIZE THE EVENT HANDLERS/STORYBOARDS:
            sbResume.Completed += sbResume_Completed;
            sbContinue.Completed += sbContinue_Completed;
            sbGame.Completed += sbGame_Completed;
            sbUser.Completed += sbUser_Completed;
            sbHardRectRemover.Completed += sbHardRectRemover_Completed;
            GameGrid.Loaded += LoadGame;
            MessageGrid.Tap += MessageGrid_Tap;
            Storyboard.SetTarget(hidePauseGrid, MessageGrid);// FADE OUT WHEN TAPPED

            // INITIALIZE OUR TIMERS:
            delayTimer = new DispatcherTimer();
            delayTimer.Tick += delayTimer_Tick;
            delayTimer.Interval = new TimeSpan(0, 0, 1);

            // SET UP OUR USER TIMER (STARTS ONCE WE ARE WAITING ON THEM TO BEGIN TAPPING THE NEXT SEQUENCE):
            userTimer = new DispatcherTimer();
            userTimer.Tick += userTimer_Tick;
            userTimer.Interval = new TimeSpan(0, 0, 1);

            // FOR LIGHT THEMES, WE HAVE TO FIX A FEW THINGS
            // THAT THE THEMEMANAGER DOESN'T SET CORRECTLY:
            if (Globals.Data.Theme == Enumerations.Theme.Light) ApplyLightTheme();
            
        }

        /// <summary>
        /// WHEN THE BACK BUTTON IS PRESSED, WE WANT TO PAUSE THE GAME AND NAVIGATE 
        /// BACK TO THE MENU PAGE:
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            userTimer.Stop();
            GameHelper.PauseGame(true);
            NavigationService.Navigate(new Uri("/Pages/Menu.xaml", UriKind.Relative));
        }

        /// <summary>
        /// EXECUTES AFTER THE PAGE IS LOADED:
        /// </summary>
        private void LoadGame(object sender, EventArgs e)
        {

            // DISABLE THE RECTANGLES IMMEDIATELY:
            DisableRectangles();

            // LOAD UP GAME COLORS:
            efGameColor.Value = Globals.Data.GameColor;
            efUserColor.Value = Globals.Data.UserColor;

            // SETUP ADVERTISEMENT:
            if (Globals.bDisplayAds) Advertising.GetAdForGrid(LayoutRoot, 4, 0, 4);

            if (Globals.Data.GameMode == Enumerations.GameMode.Easy)
            {
                // FOR EASY MODE, ONLY USE 4 RECTANGLE:
                Globals.numberOfRectangles = 4;

                if (Globals.bGameIsPaused)
                    RemoveHardRectanglesWithoutAnimation();
                else
                    AnimateRemovalOfHardRectangles();
            }
            else
            {
                // FOR HARD MODE, USE 8 RECTANGLES:
                Globals.numberOfRectangles = 8; 
            }

            if (Globals.bGameIsPaused)
            {
                // GAME IS PAUSED, SO NOW WE HAVE TO DISPLAY THE PAUSE CANVAS:
                // SAVE THE CURRENT SEQUENCE COUNTER:
                Globals.nSequenceCounter = (int)PhoneApplicationService.Current.State["MemorySquared_SequenceCounter"];
                ShowPauseGrid();
                
            } else {
                
                // RESET COUNTDOWN:
                nGameCountdown = 3;

                // start the 5 second timer which will begin the game:
                gameStartTimer = new DispatcherTimer();
                gameStartTimer.Tick += StartGame;
                gameStartTimer.Interval = new TimeSpan(0, 0, 1);
                gameStartTimer.Start();

                lblCountdown.Visibility = System.Windows.Visibility.Visible;
            }
            
        }

        private void StartGame(object sender, EventArgs e)
        {

            if (nGameCountdown == 0)
            {
                // WAIT ONE MORE SECOND:
                lblCountdown.Visibility = System.Windows.Visibility.Collapsed;
                nGameCountdown = -1;
                return;
            }
            else if (nGameCountdown > 0)
            {
                // update timer label:
                lblCountdown.Text = nGameCountdown.ToString();
                nGameCountdown = nGameCountdown - 1;
                return;
            }
                        
            // game is starting!
            gameStartTimer.Stop();
            gameStartTimer = null; 

            Globals.nSequenceCounter = 0;

            // instantiate our lists:
            Globals.currentSequence = new List<int>();
            Globals.userTapSequence = new List<int>();

            // THE GAME IS NOW IN PROGRESS:
            Globals.bGameIsStarted = true;

            ShowNextRandomRect();

        }

        private void AnimateRemovalOfHardRectangles()
        {
            // REMOVE THE TOP 2 AND BOTTOM 2 RECTANGLES:
            RectsToRemove.Push(Rect5);
            RectsToRemove.Push(Rect6);
            RectsToRemove.Push(Rect7);
            RectsToRemove.Push(Rect8);

            // ANIMATE THE REMOVAL ONE BY ONE:
            TryRemoveRectangle();
        }

        private void RemoveHardRectanglesWithoutAnimation()
        {
            GameGrid.Children.Remove(Border5);
            GameGrid.Children.Remove(Border6);
            GameGrid.Children.Remove(Border7);
            GameGrid.Children.Remove(Border8);

            // RESET THE NUMBER OF RECTANGLES:
            Globals.numberOfRectangles = 4;
        }

        private void TryRemoveRectangle()
        {
            if (RectsToRemove.Count > 0)
            {
                // ANIMATE THE REMOVAL OF THIS RECTANGLE:
                Rectangle oRect = RectsToRemove.Pop();
                sbHardRectRemover.SetValue(Storyboard.TargetNameProperty, oRect.Name);
                sbHardRectRemover.Begin();
            }
            else
            {
               // RESET THE NUMBER OF RECTANGLES:
                Globals.numberOfRectangles = 4;
            }
        }

        private void sbHardRectRemover_Completed(object sender, EventArgs e)
        {

            // get the rectangle which was just used in the storyboard:
            Storyboard sb = (Storyboard)sender;
            sb.Stop();

            string sRectName = sb.GetValue(Storyboard.TargetNameProperty).ToString();

          
            // TO DO: FIGURE OUT WHY THE ANIMATION DOESN'T STAY GRAY AND
            // THEN WE WON'T HAVE TO ACTUALLY REMOVE THE RECTANGLE:
            object element = (FrameworkElement)GameGrid.FindName(sRectName);
            Rectangle oRect = (Rectangle)element;
            Border oBorder = (Border)oRect.Parent;
            GameGrid.Children.Remove(oBorder);
            
            // TRY TO RECURSIVELY REMOVE THE NEXT RECTANGLE:
            TryRemoveRectangle();
        }

        private void DisplayRectangle(int indexOfRectInSequence, bool showFromGame)
        {
            if (!Globals.bGameIsPaused)
            {
                // MAKE SURE THE GAME WASN'T DEACTIVATED AND THEN JUST NOW REACTIVATED:
                if (Deactivated() == false)
                {

                    // get the rectangle namne that cooresponds to this index:
                    if (Globals.currentSequence.Count == 0) AddRandomRectToCurrentSequence();
                    int nRectName = Globals.currentSequence[indexOfRectInSequence];
                    string sRectName = "Rect" + nRectName;

                    if (showFromGame == true)
                    {
                        // show the user a RED rectangle from the GAME:
                        sbGame.SetValue(Storyboard.TargetNameProperty, sRectName);
                        sbGame.Begin();
                    }
                    else
                    {
                        // show the user a GREEN rectangle because THEY tapped it:
                        sbUser.SetValue(Storyboard.TargetNameProperty, sRectName);
                        DisableRectangles();
                        sbUser.Begin();
                    }
                }
            }
        }

        private void ShowNextRandomRect()
        {

            // add a random rect to the collection:
            AddRandomRectToCurrentSequence();

            Globals.nSequenceCounter += 1;
            
            // pass in the last index (which is the rect we just added);
            DisplayRectangle(Globals.currentSequence.Count - 1, true);
        }

        private void AddRandomRectToCurrentSequence()
        {
            // get a random number between 1 and numberOfRectangles - this is how we'll
            // decide which rectangle to light up next:
            Random random = new Random();
            int nRectNumber = random.Next(1, Globals.numberOfRectangles);

            // add this rectangle to our current sequence:
            Globals.currentSequence.Add(nRectNumber);
        }

        private void Rectangle_Tap(object sender, EventArgs e)
        {
                
            // USER TAPPED THIS RECTANGLE:
            Rectangle rect = (Rectangle)sender;
                                               
            // THE TAG PROPERTY HAS THE NUMBER OF THE RECTANGLE:
            int rectNumber = Convert.ToInt32(rect.Tag);

            // IGNORE THIE TAP IF IT WOUDL ADD TOO MANY TO OUR LIST (BUG?):
            if (Globals.userTapSequence.Count + 1 > Globals.currentSequence.Count) return;

            // ADD THIS RECTANGLE TO THE SEQUENCE OF RECTANGLES WHICH THE USER HAS TAPPED: 
            Globals.userTapSequence.Add(rectNumber);

            // IF THIS WAS THE LAST RECTANGLE IN THE SEQUENCE, DISABLE THEM IMMEDIATELY.
            // OTHERWISE AN EXCEPTION COULD BE THROWN IF WE ALLOW THE USER TO TAP TOO MANY RECTANGLES:
            if (Globals.userTapSequence.Count == Globals.currentSequence.Count) DisableRectangles();

            // is this number the next one in the list?
            if (rectNumber != Globals.currentSequence[Globals.userTapSequence.Count - 1])
            {

                // WRONG ANSWER: 

                // VIBRATE TO LET THE USER KNOW:
                VibrationDevice.GetDefault().Vibrate(TimeSpan.FromSeconds(0.5));
                
                // STOP THE USER TIMER:
                userTimer.Stop();

                // SET THE INCORRECT FLAG:
                Globals.bIncorrect = true;

                // CHECK IF THEY BEAT THEIR HIGHEST SCORE:
                int nCurrentHighScore = 0;
                int nNewScore = Globals.nSequenceCounter - 1; // they didn't get the last sequence right, subtract one

                if (Globals.Data.GameMode == Enumerations.GameMode.Easy)
                    nCurrentHighScore = Globals.Data.HighScoreEasy;
                else
                    nCurrentHighScore = Globals.Data.HighScoreHard;

                // get scores ready to pass into the "Incorrect" page:
                PhoneApplicationService.Current.State["MemorySquared_OldScore"] = nCurrentHighScore;
                PhoneApplicationService.Current.State["MemorySquared_NewScore"] = nNewScore;

                // RESET CURRENT SEQUENCE:
                Globals.currentSequence = new List<int>();
                Globals.iCurrentlyDisplayingSequenceIndex = 0;

                // TAKE USER TO THE "INCORRECT" PAGE:
                NavigationService.Navigate(new Uri("/Pages/Incorrect.xaml", UriKind.Relative));
            }
            else
            {
                // correct answer:
                DisplayRectangle(Globals.userTapSequence.Count - 1, false);
            }
        }

        void sbGame_Completed(object sender, EventArgs e)
        {
            Storyboard sb = (Storyboard)sender;
            sb.Stop();

            // did the user tap a rectangle or are we currently
            // displaying the sequence to them?
            if (Globals.bCurrentlyDisplayingSequence == true)
            {

                // we're currently displaying the sequence to the user
                if (Globals.iCurrentlyDisplayingSequenceIndex < Globals.currentSequence.Count - 1)
                {

                    // not done yet!
                    Globals.iCurrentlyDisplayingSequenceIndex += 1;
                    DisplayRectangle(Globals.iCurrentlyDisplayingSequenceIndex, true);

                }
                else
                {
                    // done!
                    Globals.bCurrentlyDisplayingSequence = false;

                    //re-enable the rectangles:
                    EnableRectangles();

                    // when resuming the game after it was paused, 
                    // we don't want to display another random rectangle,
                    // because we need the user to complete THIS sequence first
                    if (Globals.bJustResumedGame == false)
                    {
                        // now add a random on to the list:
                        ShowNextRandomRect();
                    }
                    else
                    {
                        // since we aren't calling ShowNextRandomRect,
                        // we need to restart the user-wait timer manually:
                        //Globals.waitForUserTimer.Start();
                        Globals.bJustResumedGame = false;
                    }
                }

            }
            else
            {
                // should only hit here after we show them the VERY FIRST rectangle

                // now we are going to wait for the user to match the sequence!
                if (Globals.Data.YourTurnHelper == true && Globals.currentSequence.Count == 1)
                {
                    // THE USER IS DONE, DISPLAY THE NEW SEQUENCE:

                    // If the user hits back during the 3-2-1 countdown, the game will be paused before
                    // execution gets here. We don't want to re-pause here because false is passed in 
                    // which causes the sequence to not be shown upon resume.
                    if (Globals.bGameIsPaused == false)
                    {
                        GameHelper.PauseGame(false);
                        ShowYourTurnMessage();
                    }

                }else
                {
                    // WE WILL GET HERE EACH TIME AFTER THE "NEWEST" RANDOM RECT IS SHOWN:
                    userTimer.Start();

                    //re-enable the rectangles:
                    EnableRectangles();

                    //reset our user sequence:
                    Globals.userTapSequence = new List<int>();
                }
            }
        }

        // gets called once the user taps a rectangle and it finishes displaying:
        void sbUser_Completed(object sender, EventArgs e)
        {

            //re-enable the rectangles:
            EnableRectangles();

            // get the rectangle which was just used in the storyboard:
            Storyboard sb = (Storyboard)sender;
            sb.Stop();

            // WAS THIS THE LAST RECTANGLE OF THE CURRENT SEQUENCE?
            if (Globals.userTapSequence.Count == Globals.currentSequence.Count)
            {
                // WAIT ONE SECOND BEFORE WE SHOW THE NEXT SEQUENCE:
                userTimer.Stop();
                delayTimer.Start();
            }
        }

        void delayTimer_Tick(object sender, EventArgs e)
        {
            // STOP THE TIMER:
            delayTimer.Stop();
            
            // START OVER - SHOW THE  SEQUENCE TO THE USER:
            ShowSequence();
        }

        void userTimer_Tick(object sender, EventArgs e)
        {

            //EVERY SECOND WE WAIT FOR THE USER, CHECK TO MAKE SURE
            //THE GAME WASN'T DEACTIVATED:
            Deactivated();

        }

        /// <summary>
        /// CALL THIS WHEN YOU NEED TO DISPLAY THE CURRENT SEQUENCE TO THE USER
        /// </summary>
        void ShowSequence()
        {

            // display the current rectangles in the sequence (starting with the first - index 0):
            Globals.bCurrentlyDisplayingSequence = true;

            // disable the rectangles from being tapped while we show the sequence:
            DisableRectangles();

            Globals.iCurrentlyDisplayingSequenceIndex = 0; //reset
            DisplayRectangle(0, true);

        }
              

        void ShowPauseGrid()
        {
            MessageHeader.Text = AppResources.Paused;
            MessageContent.Text = AppResources.UponResuming;
            MessageFooter.Text = "(" + AppResources.TapToResume + ")";

            MessageGrid.Visibility = System.Windows.Visibility.Visible;
        }

        void MessageGrid_Tap(object sender, EventArgs e)
        {
            // SWITCH THE SWITCHES:
            Globals.bGameIsPaused = false;
            Globals.bCurrentlyDisplayingSequence = true;

            // ENABLE THE RECTANGLES FOR TAPPING:
            EnableRectangles();

            // GET THE CURRENT SEQUENCE:
            Globals.currentSequence = (List<int>)PhoneApplicationService.Current.State["MemorySquared_CurrentSequence"];

            // START THE USER WAIT TIMER:
            userTimer.Start();
         
            // HIDE THE PAUSE CANVAS:
            sbResume.Begin();
        }

        void sbResume_Completed(object sender, EventArgs e)
        {
            // HIDE THE PAUSE GRID:
            MessageGrid.Visibility = System.Windows.Visibility.Collapsed;

            // CLEAR VALUES:
            MessageHeader.Text = "";
            MessageContent.Text = "";
            MessageFooter.Text = "";

            // STOP THE STORYBOARD:
            Storyboard sb = (Storyboard)sender;
            sb.Stop();
            
            // DISPLAY THE LAST SEQUENCE TO THE USER:
            if(Globals.bShowSequenceUponResume == true)
            {
                // DON'T SHOW A RANDOM RECTANGLE AFTER WE DISPLAY THE SEQUENCE:
                Globals.bJustResumedGame = true;

                // SHOW THE SEQUENCE BACK TO THE USER:
                ShowSequence();
            }
        }

        void ShowYourTurnMessage()
        {
            MessageHeader.Text = AppResources.YourTurn;
            MessageContent.Text = AppResources.YourTurnMessage;
            MessageFooter.Text = "(" + AppResources.DisablePopup + ")";

            MessageGrid.Visibility = System.Windows.Visibility.Visible;

        }

        void sbContinue_Completed(object sender, EventArgs e)
        {

            MessageGrid.Visibility = System.Windows.Visibility.Collapsed;

            // STOP THE STORYBOARD:
            Storyboard sb = (Storyboard)sender;
            sb.Stop();
        }

        void DisableRectangles()
        {
            Rect1.IsHitTestVisible = false;
            Rect2.IsHitTestVisible = false;
            Rect3.IsHitTestVisible = false;
            Rect4.IsHitTestVisible = false;
            Rect5.IsHitTestVisible = false;
            Rect6.IsHitTestVisible = false;
            Rect7.IsHitTestVisible = false;
            Rect8.IsHitTestVisible = false;
        }

        void EnableRectangles()
        {
            Rect1.IsHitTestVisible = true;
            Rect2.IsHitTestVisible = true;
            Rect3.IsHitTestVisible = true;
            Rect4.IsHitTestVisible = true;
            Rect5.IsHitTestVisible = true;
            Rect6.IsHitTestVisible = true;
            Rect7.IsHitTestVisible = true;
            Rect8.IsHitTestVisible = true;
        }

        bool Deactivated()
        {
            if (Globals.bDeactivated == true)
            {
                // RESET:
                Globals.bDeactivated = false;

                // DISABLE RECTANGLES FROM BEING TAPPED:
                DisableRectangles();

                // PAUSE THE GAME: 
                GameHelper.PauseGame(true);

                // SHOW THE PAUSE GRID:
                ShowPauseGrid();

                // RETURN THAT THE GAME IS/WAS DEACTIVATED:
                return true;
            }
            else
            {
                // GAME HAS NOT BEEN DEACTIVATED:
                return false;
            }

        }

        void ApplyLightTheme()
        {
            // RESET THE BORDERS:
            Border1.Style = LightBorderStyle;
            Border2.Style = LightBorderStyle;
            Border3.Style = LightBorderStyle;
            Border4.Style = LightBorderStyle;
            Border5.Style = LightBorderStyle;
            Border6.Style = LightBorderStyle;
            Border7.Style = LightBorderStyle;
            Border8.Style = LightBorderStyle;

            // NOW RESET THE RECTANGLE THEMES:
            Rect1.Style = LightRectangleStyle;
            Rect2.Style = LightRectangleStyle;
            Rect3.Style = LightRectangleStyle;
            Rect4.Style = LightRectangleStyle;
            Rect5.Style = LightRectangleStyle;
            Rect6.Style = LightRectangleStyle;
            Rect7.Style = LightRectangleStyle;
            Rect8.Style = LightRectangleStyle;

            // UPDATE THE STORYBOARDS:
            GameRectFromColor.Value = Colors.Transparent;
            UserRectToColor.Value = Colors.Transparent;
            HardRectToRemoveStartColor.Value = Colors.Transparent;
            HardRectToRemoveEndColor.Value = Colors.Transparent;

        }

        private void NewGame()
        {
            Globals.nSequenceCounter = 0;
            Globals.userTapSequence = new List<int>();
            Globals.bCurrentlyDisplayingSequence = false;
        }

    }
}