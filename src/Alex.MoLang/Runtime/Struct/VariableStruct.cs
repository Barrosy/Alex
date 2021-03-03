using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Alex.MoLang.Runtime.Exceptions;
using Alex.MoLang.Runtime.Value;
using NLog;

namespace Alex.MoLang.Runtime.Struct
{
	public class VariableStruct : IMoStruct
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger(typeof(VariableStruct));
		public readonly Dictionary<string, IMoValue> Map = new Dictionary<string, IMoValue>();

		/// <inheritdoc />
		public object Value => Map;

		public VariableStruct()
		{
			
		}

		public VariableStruct(IEnumerable<KeyValuePair<string, IMoValue>> values)
		{
			Map = new Dictionary<string, IMoValue>(values);
		}

		/// <inheritdoc />
		public virtual void Set(string key, IMoValue value)
		{
			Queue<string> segments = new Queue<string>(key.Split("."));
			string        main     = segments.Dequeue();

			if (segments.Count > 0 && main != null) {
				//object vstruct = Get(main, MoParams.Empty);

				if (!Map.TryGetValue(main, out var container)) {
					Map.TryAdd(main, container = new VariableStruct());
					//	throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
				}
				
				if (container is IMoStruct moStruct)
				{
					moStruct.Set(string.Join(".", segments), value);
				}
				else
				{
					throw new MoLangRuntimeException($"Variable was not a struct: {key}", null);
				}
				
				//((IMoStruct) vstruct).Set(string.Join(".", segments), value);

				//Map[main] = (IMoStruct)vstruct;//.Add(main, (IMoStruct) vstruct);
			} else
			{
				Map[key] = value;
				//Map.Add(key, value);
			}
		}

		/// <inheritdoc />
		public virtual IMoValue Get(string key, MoParams parameters)
		{
			Queue<string> segments = new Queue<string>(key.Split("."));
			var           main     = segments.Dequeue();
			
			if (segments.Count > 0 && main != null)
			{
				IMoValue value = null;//Map[main];
				if (!Map.TryGetValue(main, out value))
					return DoubleValue.Zero;

				if (value is IMoStruct moStruct)
				{
					return moStruct.Get(string.Join(".", segments), parameters);
				}
			}

			if (Map.TryGetValue(key, out var v))
				return v;

			//Log.Info($"Unknown variable: {key}");
			return DoubleValue.Zero;
			return Map[key];
		}

		/// <inheritdoc />
		public void Clear()
		{
			Map.Clear();
		}
	}
}