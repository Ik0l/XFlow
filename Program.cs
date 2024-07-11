namespace Cleanup
{
    internal class Program
    {
        private const double TargetChangeTime = 1;

        private double _previousTargetSetTime;
        private bool _isTargetSet;
        private object _lockedCandidateTarget;
        private object _lockedTarget;
        private object _target;
        private object _previousTarget;
        private object _activeTarget;
        private object _targetInRangeContainer;

        public void CleanupTest(Frame frame)
        {
            try
            {
                ClearNonTarget(ref _lockedCandidateTarget);
                ClearNonTarget(ref _lockedTarget);

                _isTargetSet = false;
				// Sets _activeTarget field
                TrySetActiveTargetFromQuantum(frame);

                // If target exists and can be targeted, it should stay within Target Change Time since last target change
                if (IsValidTarget(_target) && IsWithinTargetChangeTime())
                {
                    _isTargetSet = true;
                }
                _previousTarget = _target;

                if (_isTargetSet)
                {
                    return;
                }
                
                _target = GetValidTarget();
                _isTargetSet = _target != null;
            }
            finally
            {
                if (_isTargetSet && _previousTarget != _target)
                {
                    _previousTargetSetTime = Time.time;
                }
                
                TargetableEntity.Selected = _target;
            }
        }
        
        private void ClearNonTarget(ref object target)
        {
            if (IsValidTarget(target, false))
            {
                target = null;
            }
        }
        
        private bool IsValidTarget(object target, bool canBeTarget = true)
        {
            return target != null && target.CanBeTarget == canBeTarget;
        }
        
        private bool IsWithinTargetChangeTime()
        {
            return Time.time - _previousTargetSetTime < TargetChangeTime;
        }
        
        private object GetValidTarget()
        {
            return IsValidTarget(_lockedTarget) ? _lockedTarget :
                IsValidTarget(_activeTarget) ? _activeTarget :
                _targetInRangeContainer.GetTarget();
        }
    }
}