using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Alex.MoLang.Runtime.Struct;
using Alex.MoLang.Runtime.Value;

namespace Alex.MoLang.Runtime
{
	public class MoLangEnvironment : IMoValue
	{
		/// <inheritdoc />
		public object Value => this;
		
		public ConcurrentDictionary<string, IMoStruct> Structs { get; } = new ConcurrentDictionary<string, IMoStruct>();

		public IMoValue GetValue(string name) {
			return GetValue(name, MoParams.Empty);
		}

		public IMoValue GetValue(string name, MoParams param) {
			Queue<string> segments = new Queue<string>(name.Split("."));
			string        main     = segments.Dequeue();

			if (Structs.ContainsKey(main)) {
				return Structs[main].Get(string.Join(".", segments), param);
			}

			return new DoubleValue(0.0);
		}

		public void SetValue(String name, IMoValue value) {
			Queue<string> segments = new Queue<string>(name.Split("."));
			string        main     = segments.Dequeue();

			if (Structs.ContainsKey(main)) {
				Structs[main].Set(string.Join(".", segments), value);
			}
		}
	}
}