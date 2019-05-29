using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
	/// <summary>
	/// Eventually we may want to expand this to support multiple entities activated at once...
	/// </summary>
	public class ActivationManager
	{
		private List<IActivatable> _activatables;
		private IActivatable _active;
		private IActivatable _previous;

		private IActivatable Active
		{
			get { return _active; }
			set {
				if (_active != value)
				{
					_previous = _active;
					_active = value;
				}
			}
		}

		public ActivationManager()
		{
			_activatables = new List<IActivatable>();
			_active = null;
			_previous = null;
		}

		public void Add(IActivatable obj)
		{
			_activatables.Add(obj);
		}

		public void Activate(IActivatable obj)
		{
			if ((this.Active == obj) || (obj == null))
				return;
		
			_activatables.ForEach(x => x.IsActive = (x == obj));
			// This doesn't actually check that obj was eveen in the managed collection...
			this.Active = obj;
		}

		public void Deactivate(IActivatable obj, bool activatePrevious = true)
		{
			if (_active != obj)
				return;

			if (activatePrevious)
			{
				Activate(_previous);
			}
			else if (this.Active != null)
			{
				this.Active.IsActive = false;
				this.Active = null;
			}
		}
	}
}
