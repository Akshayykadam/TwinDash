﻿//----------------------------------------------
// Flip Web Apps: Beautiful Transitions
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

using System;
using System.Collections;
using BeautifulTransitions.Scripts.Helper;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses
{
    /// <summary>
    /// Base class for all transition steps
    /// </summary>
    public class TransitionStep {

        /// <summary>
        /// Defines what time source is used to update transitions
        /// </summary>
        public enum TimeUpdateMethodType
        {
            /// <summary>
            /// Update is based on Time.time. Use this for transitions that need to be synchronized on gameplay, and that need to be paused when the game is paused.
            /// </summary>
            GameTime,
            /// <summary>
            /// Update is based on Time.unscaledTime. Use this for transitions that need to be updated even when gameplay is paused. Example: Menus transitions need to be updated even when the game is paused.
            /// </summary>
            UnscaledGameTime
        };

        /// <summary>
        /// The coordinate mode that is to be used.
        /// </summary>
        public enum CoordinateSpaceType        {
            Global,
            Local,
            AnchoredPosition
        };

        /// <summary>
        /// The transition mode
        /// </summary>
        /// Note that this is not used by the base class as we have no way of knowing what type of value we are referring
        /// or indeed whether things like Original / Current make sense (e.g. non singleton targets). As such
        /// override Start() in your TransitionStep implementation and set values as appropriate.
        public enum TransitionModeType
        {
            Specified,
            ToOriginal,
            FromCurrent,
            FromOriginal,
            ToCurrent
        };


        /// <summary>
        /// The loop mode
        /// </summary>
        public enum LoopModeType
        {
            /// <summary>
            /// Play once and don't loop (default)
            /// </summary>
            None,
            /// <summary>
            /// Loop, playing again from the start
            /// </summary>
            Loop,
            /// <summary>
            /// Ping pong back and forth
            /// </summary>
            PingPong
        };

        #region Transition Settings

        /// <summary>
        /// The target GameObject (not used by all transition types)
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Normalised progress of the current transtion between 0 and 1 in a linear (constant) fashion.
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// A tweened version of the progress (range 0..1) based upon the TweenType.
        /// </summary>
        /// This value is typically in the range 0 to 1, but may fall outside these bounds depending on the 
        /// tween type. You can use this as a normalised tweened value to allow for simpler and more 
        /// performant calculation of other tweened values (see ValueFromProgressTweened).
        public float ProgressTweened { get; private set; }
        
        /// <summary>
        /// Time in seconds before this transition should be started.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        /// How long this transition will / should run for.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// The easing function.
        /// </summary>
        public TransitionHelper.TweenType TweenType
        {
            get {
                return _tweenType;
            }
            set {
                _tweenType = value;

                // Get the easing function - will be null for unsupported types.
                _tweenFunction = TransitionHelper.GetTweenFunction(TweenType);
            }
        }
        TransitionHelper.TweenType _tweenType;


        /// <summary>
        /// Controls how time is incremented when playing the transition.
        /// </summary>
        public TimeUpdateMethodType TimeUpdateMethod { get; set; }

        /// <summary>
        /// The looping mode for the transition
        /// </summary>
        public LoopModeType LoopMode { get; set; }

        /// <summary>
        /// How the transition should be run.
        /// </summary>
        public TransitionModeType TransitionMode { get; set; }

        /// <summary>
        /// An optional custom curve to show how the transition should be run.
        /// </summary>
        public AnimationCurve AnimationCurve { get; set; }

        /// <summary>
        /// The coordinate mode that should be used
        /// </summary>
        public CoordinateSpaceType CoordinateSpace { get; set; }

        /// <summary>
        /// When chaining transitions this can be used for setting the parent.
        /// </summary>
        public TransitionStep Parent { get; set; }

        #endregion Transition Settings

        #region Callbacks

        /// <summary>
        /// Action called when a transition starts
        /// </summary>
        public Action<TransitionStep> OnStart { get; set; }

        /// <summary>
        /// Action called when an update occurrs - TransitionStep passed as a parameter
        /// </summary>
        public Action<TransitionStep> OnUpdate { get; set; }

        /// <summary>
        /// Action called when this transition completes
        /// </summary>
        public Action<TransitionStep> OnComplete { get; set; }

        /// <summary>
        /// Custom data that is passed to OnCompleteObject when this transition completes
        /// </summary>
        public object UserData { get; set; }

        #endregion Callbacks

        #region Flags

        /// <summary>
        /// True when the transition is stopped
        /// </summary>
        public bool IsStopped { get; protected set; }

        /// <summary>
        /// True when the transition is paused
        /// </summary>
        public bool IsPaused { get; protected set; }

        #endregion Flags

        TweenMethods.TweenFunction _tweenFunction;

        #region Constructors

        /// <summary>
        /// Constructor. Semi-internal - If you use this be prepared for changes. The best way is to use named 
        /// parameters for all optional arguments!
        /// </summary>
        /// <param name="target"></param>
        /// <param name="delay"></param>
        /// <param name="duration"></param>
        /// <param name="transitionMode"></param>
        /// <param name="timeUpdateMethod"></param>
        /// <param name="tweenType"></param>
        /// <param name="animationCurve"></param>
        /// <param name="coordinateSpace"></param>
        /// <param name="onStart"></param>
        /// <param name="onUpdate"></param>
        /// <param name="onComplete"></param>
        public TransitionStep(UnityEngine.GameObject target = null,
            float delay = 0,
            float duration = 0.5f,
            TransitionModeType transitionMode = TransitionModeType.Specified,
            TimeUpdateMethodType timeUpdateMethod = TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            CoordinateSpaceType coordinateSpace = CoordinateSpaceType.Global,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null, 
            Action<TransitionStep> onComplete = null)
        {
            Target = target;
            Delay = delay;
            Duration = duration;
            TransitionMode = transitionMode;
            TimeUpdateMethod = timeUpdateMethod;
            TweenType = tweenType;
            AnimationCurve = animationCurve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
            CoordinateSpace = coordinateSpace;

            AddOnStartAction(onStart);
            AddOnUpdateAction(onUpdate);
            AddOnCompleteAction(onComplete);
        }

        #endregion Constructors

        #region Chaining support

        /// <summary>
        ///  Set the delay before teh transition starts
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public TransitionStep SetDelay(float delay)
        {
            Delay = delay;
            return this;
        }

        /// <summary>
        ///  Set the duration that the transition should run for
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public TransitionStep SetDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        /// <summary>
        /// Set the transition type
        /// </summary>
        /// <param name="tweenType"></param>
        /// <returns></returns>
        public TransitionStep SetTweenType(TransitionHelper.TweenType tweenType)
        {
            TweenType = tweenType;
            return this;
        }

        /// <summary>
        /// Set the time update method
        /// </summary>
        /// <param name="timeUpdateMethod"></param>
        /// <returns></returns>
        public TransitionStep SetTimeUpdateMethod(TimeUpdateMethodType timeUpdateMethod)
        {
            TimeUpdateMethod = timeUpdateMethod;
            return this;
        }

        /// <summary>
        /// Set the loop mode
        /// </summary>
        /// <param name="loopMode"></param>
        /// <returns></returns>
        public TransitionStep SetLoopMode(LoopModeType loopMode)
        {
            LoopMode = loopMode;
            return this;
        }

        /// <summary>
        /// Set the transition mode
        /// </summary>
        /// <param name="transitionMode"></param>
        /// <returns></returns>
        public TransitionStep SetTransitionMode(TransitionModeType transitionMode)
        {
            TransitionMode = transitionMode;
            return this;
        }

        /// <summary>
        /// Set the animation curve for this transition
        /// </summary>
        /// <param name="animationCurve"></param>
        /// <returns></returns>
        public TransitionStep SetAnimationCurve(AnimationCurve animationCurve)
        {
            AnimationCurve = animationCurve;
            return this;
        }

        /// <summary>
        /// Set the coordinate mode for this transition (see individual items for the exact modes supported for each item)
        /// </summary>
        /// <param name="coordinateMode"></param>
        /// <returns></returns>
        public TransitionStep SetCoordinateMode(CoordinateSpaceType coordinateMode)
        {
            CoordinateSpace = coordinateMode;
            return this;
        }

        /// <summary>
        /// Add an OnStart action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TransitionStep AddOnStartAction(Action<TransitionStep> action)
        {
            OnStart += action;
            return this;
        }

        /// <summary>
        /// Add an OnUpdate action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TransitionStep AddOnUpdateAction(Action<TransitionStep> action)
        {
            OnUpdate += action;
            return this;
        }
        /// <summary>
        /// Add an OnComplete action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TransitionStep AddOnCompleteAction(Action<TransitionStep> action)
        {
            OnComplete += action;
            return this;
        }

        /// <summary>
        /// Add an OnComplete action. with a parameter for user data.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TransitionStep AddOnCompleteAction(Action<TransitionStep> action, object userData)
        {
            OnComplete += action;
            UserData = userData;
            return this;
        }

        /// <summary>
        /// Add a TransitionStep that should be invoked in OnStart.
        /// </summary>
        /// <param name="transitionStep"></param>
        /// <returns></returns>
        public TransitionStep AddOnStartTransitionStep(TransitionStep transitionStep)
        {
            if (transitionStep != null)
                OnStart += transitionStep.Start;
            return this;
        }

        /// <summary>
        /// Add a TransitionStep that should be invoked in OnComplete.
        /// </summary>
        /// <param name="transitionStep"></param>
        /// <returns></returns>
        public TransitionStep AddOnCompleteTransitionStep(TransitionStep transitionStep)
        {
            if (transitionStep != null)
                OnComplete += transitionStep.Start;
            return this;
        }

        /// <summary>
        /// Chain a new custom transition step with the specfied settings onto the current one.
        /// </summary>
        /// <returns></returns>
        public TransitionStep ChainCustomTransitionStep(float delay = 0,
            float duration = 0.5f,
            TransitionStep.TransitionModeType transitionMode = TransitionStep.TransitionModeType.Specified,
            TransitionStep.TimeUpdateMethodType timeUpdateMethod = TransitionStep.TimeUpdateMethodType.GameTime,
            TransitionHelper.TweenType tweenType = TransitionHelper.TweenType.linear,
            AnimationCurve animationCurve = null,
            bool runAtStart = false,
            Action<TransitionStep> onStart = null,
            Action<TransitionStep> onUpdate = null,
            Action<TransitionStep> onComplete = null)
        {
            var newTransitionStep = new TransitionStep(Target, 
                delay,
                duration,
                transitionMode,
                timeUpdateMethod,
                tweenType,
                animationCurve,
                onStart: onStart,
                onUpdate: onUpdate,
                onComplete: onComplete);
            newTransitionStep.AddToChain(this, runAtStart);
            return newTransitionStep;
        }

        /// <summary>
        /// For any chained transition steps get the root transition step.
        /// </summary>
        /// <returns></returns>
        public TransitionStep GetChainRoot()
        {
            var transitionStep = this;
            while (transitionStep.Parent != null)
                transitionStep = transitionStep.Parent;
            return transitionStep;
        }

        /// <summary>
        /// Add this TransitionStep into a chain
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="runAtStart"></param>
        public void AddToChain(TransitionStep parent, bool runAtStart)
        {
            if (runAtStart)
                parent.AddOnStartTransitionStep(this);
            else
                parent.AddOnCompleteTransitionStep(this);
            Parent = parent;
        }

        #endregion Chaining support

        #region Methods for triggering life cycle states

        /// <summary>
        /// Call to start the transition
        /// </summary>
        public virtual void Start()
        {
            // clear any previous state flags
            IsStopped = false;
            IsPaused = false;

            // Run the transition - we do this on transition controller so that it can outlive scene changes if needed
            TransitionController.Instance.StartCoroutine(TransitionLoop());
        }

        /// <summary>
        /// Call to start the transition - for use from callbacks.
        /// </summary>
        public virtual void Start(TransitionStep transitionStep)
        {
            Start();
        }

        /// <summary>
        /// Reset parameters to their default state.
        /// </summary>
        //void Reset()
        //{
        //    Progress = 0;
        //    Duration = 0.5f;
        //    TweenType = TransitionHelper.TweenType.linear;
        //    AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        //}


        /// <summary>
        /// Call to stop the transtion. When a transition is stopped in this fashion, no further notifications such as
        /// OnComplete will be sent out.
        /// </summary>
        public virtual void Stop()
        {
            IsStopped = true;
        }


        /// <summary>
        /// Call to pause the transition.
        /// </summary>
        public virtual void Pause()
        {
            IsPaused = true;
        }


        /// <summary>
        /// Call to resume a paused transition.
        /// </summary>
        public virtual void Resume()
        {
            IsPaused = false;
        }


        /// <summary>
        /// Call to immediately set a transition to its end state.
        /// </summary>
        /// When the processing coroutine is next called, completion notifications will be sent out to any listeners.
        public virtual void Complete()
        {
            SetProgressToEnd();
        }

        #endregion Methods for triggering life cycle states

        #region Life cycle 
        /// <summary>
        /// Coroutine to handle the transition
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator TransitionLoop()
        {
            // if delay and duration are both zero then just set to end state, otherwise set to start and transition
            if (Mathf.Approximately(Delay + Duration, 0))
            {
                SetProgressToEnd();
                TransitionStarted();
            }
            else
            {
                SetProgressToStart();
                TransitionStarted();

                // handle any delay
                if (!Mathf.Approximately(Delay, 0))
                {
                    var delayTime = 0f;
                    while (delayTime < Delay) {
                        // only update if not paused
                        if (!IsPaused)
                            delayTime += (TimeUpdateMethod == TimeUpdateMethodType.GameTime ? Time.deltaTime : Time.unscaledDeltaTime);
                        yield return 0;
                    }
                }

                // calculate narmalised multiplication factor and avoid / by 0 error.
                var normalisedFactor = Mathf.Approximately(Duration, 0) ? float.MaxValue : (1/Duration);

                // repeat while progress is less than one.
                while (!IsStopped)
                {
                    // only update progress if not paused
                    if (!IsPaused)
                    {
                        // update progress
                        Progress += normalisedFactor * 
                            (TimeUpdateMethod == TimeUpdateMethodType.GameTime ? Time.deltaTime : Time.unscaledDeltaTime);

                        // further refine Progress baced upon the different loop modes.
                        if (LoopMode == LoopModeType.Loop && Progress >= 1)
                        {
                            Progress = 0;
                        }

                        if (LoopMode == LoopModeType.PingPong && Progress >= 1)
                        {
                            normalisedFactor *= -1;
                            Progress = 1;
                        }

                        if (LoopMode == LoopModeType.PingPong && Progress <= 0)
                        {
                            normalisedFactor *= -1;
                            Progress = 0;
                        }

                        // notify the progress change
                        SetProgress(Progress);

                        // exit if needed 
                        if (LoopMode == LoopModeType.None && Progress >= 1)
                        {
                            break;
                        }
                    }

                    yield return 0;
                }
            }

            // if we completed and weren't stopped
            if (Mathf.Approximately(Progress, 1) && !IsStopped)
            {
                TransitionCompleted();
            }
        }


        /// <summary>
        /// Called when the transition is started.
        /// </summary>
        /// This is always called when the transition starts and before any delay.
        /// If overriding then be sure to call the base class.
        protected virtual void TransitionStarted()
        {
            if (OnStart != null)
                OnStart(this);
        }


        /// <summary>
        /// Called when the transition is completed.
        /// </summary>
        /// This is called whan the transition completes assuming it hasn't been explicitely stopped.
        /// If overriding then be sure to call the base class.
        protected virtual void TransitionCompleted()
        {
            if (OnComplete != null)
                OnComplete(this);
        }



        /// <summary>
        /// Set progress to the start
        /// </summary>
        /// Note: This can be called either before any delay (most common case) or once the delay is finished.
        public void SetProgressToStart()
        {
            SetProgress(0);
        }


        /// <summary>
        /// Set progress to the end
        /// </summary>
        public void SetProgressToEnd()
        {
            SetProgress(1);
        }


        /// <summary>
        /// Set the progress restricted to the range 0 - 1. This will call the ProgressUpdated method to notify subclasses
        /// of the update and finally trigger any OnUpdate listeners.
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgress(float progress)
        {
            // catch any exception - this might occur if a referenced gameobject is destroyed before the transition completed.
            try
            {
                Progress = Mathf.Max(0, Mathf.Min(1, progress));
                ProgressTweened = ValueFromProgress(0, 1);
                ProgressUpdated();
                if (OnUpdate != null)
                    OnUpdate(this);
            }
            catch (Exception)
            {
                // if an exception then stop immediately. Don't notify incase receivers are distroyed.
                Stop();
            }
        }


        /// <summary>
        /// Override this if you need to update based upon the progress (0..1)
        /// </summary>
        protected virtual void ProgressUpdated()
        {
        }

        #endregion Life cycle 

        #region Tween Helper Methods

        /// <summary>
        /// Returns the tweened normalised progress value within the given range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected float ValueFromProgressTweened(float start, float end)
        {
            return (ProgressTweened * (end - start)) + start;
        }

        /// <summary>
        /// Based upon the current setup, evaluate a float between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected float ValueFromProgress(float start, float end)
        {
            if (TweenType == TransitionHelper.TweenType.AnimationCurve)
            {
                return ValueFromProgressAnimationCurve(start, end);
            }
            else if (_tweenFunction != null)
            {
                return _tweenFunction(start, end, Progress);
            }
            return end;
        }

        /// <summary>
        /// For a given animation curve, evaluate the value between start and end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        float ValueFromProgressAnimationCurve(float start, float end)
        {
            var curveStart = AnimationCurve.keys[0].time;
            var curveLength =
                AnimationCurve.keys[AnimationCurve.keys.Length - 1].time -
                curveStart;
            return start + (end - start) * AnimationCurve.Evaluate(curveStart + curveLength * Progress);
        }

        #endregion Tween Helper Methods
    }
}
