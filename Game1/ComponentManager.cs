﻿using System;
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
	public class ComponentManager
	{
		private List<Component> _components;

		public ComponentManager()
		{
			_components = new List<Component>();
		}

		public void Register(Component component)
		{
			_components.Add(component);
		}

		public void AddState(Component component, ComponentState state, bool clearOthers = false)
		{
			if (!_components.Contains(component))
				return;

			if (clearOthers)
				_components.ForEach(x => x.State &= ~state);
				
			component.State |= state;
		}

		public void SetState(Component component, ComponentState state, ComponentState? otherState)
		{
			if (!_components.Contains(component))
				return;

			foreach (var c in _components)
			{
				if (c == component)
					c.State = state;
				else if (otherState != null)
					c.State = (ComponentState)otherState;
			}
		}

		public void SetStateAll(ComponentState state, bool exclusive = false)
		{
			_components.ForEach(x => x.State = (exclusive ? state : x.State | state));
		}

		public void ClearState(Component component, ComponentState state)
		{
			if (!_components.Contains(component))
				return;

			component.State &= ~state;
		}
	}
}
