using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game1.Enum;
using Game1.Items;

namespace Game1
{
	// Verify we don't need these dynamicComponent calls to something like Reset(), etc....or do something here like if DyanmicComponent call Reset() (if isActive = false)
	public class ActivationManager
	{
		private static readonly string GeneralGroup = "__GENERAL__";

		private Dictionary<string, List<IActivatable>> _groups;

		public ActivationManager()
		{
			_groups = new Dictionary<string, List<IActivatable>>();
		}

		public void Register(Component[] components, bool isActive, string group) => Register(components, isActive, (group == null) ? null : new[] { group });

		public void Register(Component[] components, bool isActive, string[] groups = null)
		{
			foreach (var c in components)
				Register(c, isActive, groups);
		}

		public void Register(Component component, bool isActive, string group) => Register(component, isActive, (group == null) ? null : new[] { group });

		public void Register(Component component, bool isActive, string[] groups = null)
		{
			groups = groups ?? new[] { ActivationManager.GeneralGroup };

			foreach (var group in groups)
			{
				if (_groups.ContainsKey(group))
				{
					if (!_groups[group].Contains(component))
						_groups[group].Add(component);
				}
				else
				{
					_groups[group] = new List<IActivatable> { component };
				}
			}

			SetState(component, isActive);
		}

		private List<string> FindGroups(Component component)
		{
			var groups = new List<string>();

			foreach (var group in _groups)
				if (group.Value.Contains(component))
					groups.Add(group.Key);
					
			return groups;
		}

		public void SetState(Component[] components, bool isActive)
		{
			// Test for conflict...
			if (isActive && ActiveConflict(components))
				throw new Exception("Conflict with activation of multiple components in the same exclusive group");

			foreach (var c in components)
				SetState(c, isActive);
		}

		public void SetState(Component component, bool isActive)
		{
			foreach (var group in FindGroups(component))
			{
				if ((group == ActivationManager.GeneralGroup) || (!isActive))
					component.IsActive = isActive;
				else if (isActive)
					_groups[group].ForEach(c => c.IsActive = (c == component));			
			}
		}

		private bool ActiveConflict(Component[] components)
		{
			var componentGroups = new List<string>();

			foreach (var c in components)
			{
				var groups = FindGroups(c);
				groups.Remove(ActivationManager.GeneralGroup);

				if (componentGroups.Intersect(groups).Any())
					return true;

				componentGroups.AddRange(groups);
			}

			return false;
		}
	}
}
