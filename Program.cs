using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Test_lec_sel
{
	static class GlobalVars
	{
		public static string TRANSLATION_DIRECTION = "eng-kaz";
		//public static string TRANSLATION_DIRECTION = "eng-kaz";
		//public static string TRANSLATION_DIRECTION = "kaz-rus";
		//public static string TRANSLATION_DIRECTION = "rus-kaz";
		public static string CUBE_FILE = @"frecuencies.cube";
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
			// создаём куб многозначных слов
			// по сути массив объектов класса The_Table
			List<The_Table> Cube = new List<The_Table> ();
            
			// если файл с кубом не существует, создать куб и сохранить его в файл
			// если файл с кубом существует, прочитать из него данные
			if (!File.Exists (GlobalVars.CUBE_FILE)) {

				List<string> ambiguous_words = new List<string> (); // содержит многозначные слова
				List<List<string>> senses_of_ambiguous_words = new List<List<string>> (); // содержит списки смыслов многозначных слов, индексы соответствуют ambiguous_words 

				// временное заполнение вручную
				// заменить на чтение из файла со смыслами многознаых слов
				// ambiguous_words.Add("string");
				// senses_of_ambiguous_words.Add(new List<string>());
				// senses_of_ambiguous_words.Add(new List<string>());
				// senses_of_ambiguous_words[0].Add("жол");
				// senses_of_ambiguous_words[0].Add("ішек");

				string amb_line;
				int k = 0;
				char[] delimiters = { ':', ',' };

				System.IO.StreamReader amb_words_file = new  System.IO.StreamReader (@"eng.amb-words.txt");

				switch (GlobalVars.TRANSLATION_DIRECTION) {
				case "eng-kaz":
					amb_words_file = new System.IO.StreamReader (@"eng.amb-words.txt");
					break;
				case "kaz-eng":
					amb_words_file = new System.IO.StreamReader (@"kaz.amb-words.txt");
					break;
				case "kaz-rus":
					amb_words_file = new System.IO.StreamReader (@"kaz.amb-words.txt");
					break;
				case "rus-kaz":
					amb_words_file = new System.IO.StreamReader (@"rus.amb-words.txt");
					break;
				}

				while ((amb_line = amb_words_file.ReadLine ()) != null) {
					string[] tmp_array = amb_line.Split (delimiters);
					ambiguous_words.Add (tmp_array [0]);

					senses_of_ambiguous_words.Add (new List<string> ());
					for (int i = 1; i < tmp_array.Length; i++) {
						senses_of_ambiguous_words [k].Add (tmp_array [i]);
					}
					k++;
				}

				//Заполняем куб многозначных слов многозначными словами
				foreach (string word in ambiguous_words) {
					Cube.Add (new The_Table (word, senses_of_ambiguous_words [ambiguous_words.IndexOf (word)]));
				}

				// открыть 2 файла
				// прочитать по одной строке из файла
				// if (в англ. строке есть ENG)
				//  foreach (sense in kaz_senses)
				//      if (в каз. строке есть sense)
				//          приписать строку к смыслу sense

				// слова в файлах должны быть предварительно обработаны до словарной формы (основы слов) через пробел
				System.IO.StreamReader src_file = new System.IO.StreamReader (@"corpus.eng");
				System.IO.StreamReader trg_file = new System.IO.StreamReader (@"corpus.kaz");

				switch (GlobalVars.TRANSLATION_DIRECTION) {
				case "eng-kaz":
					src_file = new System.IO.StreamReader (@"corpus.eng");
					trg_file = new System.IO.StreamReader (@"corpus.kaz");
					break;
				case "kaz-eng":
					src_file = new System.IO.StreamReader (@"corpus.kaz");
					trg_file = new System.IO.StreamReader (@"corpus.eng");
					break;
				case "kaz-rus":
					src_file = new System.IO.StreamReader (@"corpus.kaz");
					trg_file = new System.IO.StreamReader (@"corpus.rus");
					break;
				case "rus-kaz":
					src_file = new System.IO.StreamReader (@"corpus.rus");
					trg_file = new System.IO.StreamReader (@"corpus.kaz");
					break;
				}

				string src_line;
				string trg_line;

				foreach (string word in ambiguous_words) {
					src_file.BaseStream.Position = 0;
					trg_file.BaseStream.Position = 0;

					while (((src_line = src_file.ReadLine ()) != null) && ((trg_line = trg_file.ReadLine ()) != null)) {
						if (src_line.Contains (word)) {
							foreach (string sense in senses_of_ambiguous_words[ambiguous_words.IndexOf(word)]) {
								if (trg_line.Contains (sense)) {
									Cube [ambiguous_words.IndexOf (word)].Add_sentence (sense, src_line);
								}
							}
						}
					}
				}

				src_file.Close ();
				trg_file.Close ();

				foreach (The_Table word in Cube)
					word.Compute_frequencies ();

				Console.WriteLine ("Выводим куб");

				foreach (The_Table table in Cube)
					table.Print_The_Table ();

				using (Stream output = File.Create (GlobalVars.CUBE_FILE)) {
					BinaryFormatter bf = new BinaryFormatter ();
					bf.Serialize (output, Cube);
				}
				Console.WriteLine ("Куб создан и сохранён в файл.");
			} else {
				using (Stream input = File.OpenRead (GlobalVars.CUBE_FILE)) {
					BinaryFormatter bf = new BinaryFormatter ();
					Cube = (List<The_Table>)bf.Deserialize (input);
				}
			}




			// обработка  строки аргументов
			string arguments = Console.ReadLine();
			// разбиваем строку на слова (^...$)
			arguments = arguments.ToLower();
			string[] args_word_units = arguments.Split (new string[] {"$ ^"}, StringSplitOptions.None);
			//string sgra = "^*I/*I$ ^pull<vblex><past>/тарт<v><tv><past>$ ^a<det><ind><sg>/$ ^string<n><sg>/жіп<n><sg>/жол<n><sg>$";

			for (int i = 0; i < args_word_units.Length; ++i) {
				if (args_word_units[i][0] != '^') {
					args_word_units[i] = '^' + args_word_units[i];
				}
				if (args_word_units[i][args_word_units[i].Length-1] != '$') {
					args_word_units[i] = args_word_units[i] + '$';
				}
			}

			// проверяем встречается ли "/" в строке параметров в анализе слов больше одного раза
			bool ambiguous_word_found = false;
			string pattern = @"(.*/.*){2,}";

			foreach (string unit in args_word_units) {
				Regex rgx = new Regex(pattern);
				Match match = rgx.Match(unit);
				if (match.Success) {
					ambiguous_word_found = true;
					break;
				}
			}

			if (ambiguous_word_found) {
				// заполняем массив слов контекста
				List<string> args_words = new List<string>();
				string tmp_string;
				foreach (string unit in args_word_units) {
					tmp_string = "";
					for (int i = 1; (unit [i] != '<' && unit [i] != '/'); ++i) {
						if (unit[i] != '*') {
							tmp_string += unit [i];
						}
					}

					args_words.Add (tmp_string);
				}

				// перебираем слова, формируя новую строку вывода
				string output_string = "";

				foreach (string unit in args_word_units) {
					Regex rgx = new Regex(pattern);
					Match match = rgx.Match(unit);
					if (!match.Success) {
						output_string += unit + ' ';
					}

					// для каждого слова с более чем одним вариантом перевода
					// выбрать один вариант
					string ambig_word = "";
					List<string> context_words = args_words;
					if (match.Success) {
						for (int i = 1; (unit [i] != '<' && unit [i] != '/'); ++i) {
							if (unit[i] != '*') {
								ambig_word += unit [i];
							}
						}
						context_words.Remove (ambig_word);

						Lex_sel sel = new Lex_sel(Cube);
						string found_sense = sel.select_sence (ambig_word, context_words.ToArray());
						if (found_sense != "") {
							tmp_string = "";
							for (int i = 0; unit [i] != '/'; ++i) {
								tmp_string += unit [i];
							}
							tmp_string += '/';

							int k = unit.IndexOf (found_sense);

							for (int i = k; (i < unit.Length && unit [i] != '/'); ++i) {
								tmp_string += unit [i];
							}
							tmp_string += '$';
						} else {
							tmp_string = unit;
						}
						output_string += tmp_string + ' ';
					}
				}
				output_string = output_string.Trim ();
				Console.WriteLine (output_string);
			}
			else {
				Console.WriteLine (arguments);
			}

/*
Работа из командной строки:
			string word_to_translate;
			int number_of_context_words;

			Console.WriteLine("Введите слово для перевода");
            word_to_translate = Console.ReadLine();
            Console.WriteLine("Введите количество слов в контексте перевода");
            number_of_context_words = Convert.ToInt32(Console.ReadLine());

            string[] context_words = new string[number_of_context_words];

            for (int i = 0; i < number_of_context_words; i++)
            {
                Console.WriteLine("Введите " + i.ToString() + " слово из контекста");
                context_words[i] = Console.ReadLine();
            }
*/
//            Lex_sel sel = new Lex_sel(Cube);

			//Console.WriteLine("Перевод слова " + word_to_translate + " в данном контексте: ");
			//Console.Write("" + sel.select_sence(word_to_translate, context_words) + "\n");

		}
	}
}
