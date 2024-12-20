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

using BeautifulTransitions.Scripts.Transitions.Components.GameObject.AbstractClasses;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps;
using BeautifulTransitions.Scripts.Transitions.TransitionSteps.AbstractClasses;
using UnityEngine;

namespace BeautifulTransitions.Scripts.Transitions.Components.GameObject
{
    [AddComponentMenu("Beautiful Transitions/GameObject + UI/Rotate Transition")]
    [HelpURL("http://www.flipwebapps.com/beautiful-transitions/")]
    public class TransitionRotate : TransitionGameObjectBase
    {
        public enum RotationModeType
        {
            Global,
            Local
        };

        public RotationModeType RotationMode = RotationModeType.Local;

        public InSettings InConfig;
        public OutSettings OutConfig;

        Vector3 _originalRotation;

        #region TransitionBase Overrides

        /// <summary>
        /// Gather any initial state - See TrtansitionBase for further details
        /// </summary>
        public override void SetupInitialState()
        {
            _originalRotation = ((Rotate)CreateTransitionStep()).OriginalValue;
        }


        /// <summary>
        /// Get an instance of the current transition item
        /// </summary>
        /// <returns></returns>
        public override TransitionStep CreateTransitionStep()
        {
            return new Rotate(Target, coordinateSpace: ConvertRotationMode());
        }

        /// <summary>
        /// Add common values to the transitionStep for the in transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStepIn(TransitionStep transitionStep)
        {
            var transitionStepRotate = transitionStep as Rotate;
            if (transitionStepRotate != null)
            {
                transitionStepRotate.StartValue = InConfig.StartRotation;
                transitionStepRotate.EndValue = _originalRotation;
                transitionStepRotate.CoordinateSpace = ConvertRotationMode();
            }
            base.SetupTransitionStepIn(transitionStep);
        }

        /// <summary>
        /// Add common values to the transitionStep for the out transition
        /// </summary>
        /// <param name="transitionStep"></param>
        public override void SetupTransitionStepOut(TransitionStep transitionStep)
        {
            var transitionStepRotate = transitionStep as Rotate;
            if (transitionStepRotate != null)
            {
                transitionStepRotate.StartValue = transitionStepRotate.GetCurrent();
                transitionStepRotate.EndValue = OutConfig.EndRotation;
                transitionStepRotate.CoordinateSpace = ConvertRotationMode();
            }
            base.SetupTransitionStepOut(transitionStep);
        }

        #endregion TransitionBase Overrides

        /// <summary>
        /// Convert custom rotation mode to standard one.
        /// </summary>
        /// <returns></returns>
        TransitionStep.CoordinateSpaceType ConvertRotationMode()
        {
            if (RotationMode == RotationModeType.Global)
                return TransitionStep.CoordinateSpaceType.Global;
            // else if (RotationMode == RotationModeType.Local)
            return TransitionStep.CoordinateSpaceType.Local;
        }

        #region Transition specific settings

        [System.Serializable]
        public class InSettings
        {
            [Tooltip("Start rotation (end at the GameObjects initial rotation).")]
            public Vector3 StartRotation = new Vector3(0, 0, 0);
        }

        [System.Serializable]
        public class OutSettings
        {
            [Tooltip("End rotation (starts at the GameObjects current position).")]
            public Vector3 EndRotation = new Vector3(0, 0, 0);
        }

        #endregion Transition specific settings

    }
}
