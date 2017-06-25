using System;
using System.Collections.Generic;

namespace Test_lec_sel
{
	/// <summary>
	/// // многозначное слово	f1	f2	f3
	/// // смысл 1				n1	n1	n1
	/// // смысл 2				n2	n2	n2
	/// </summary>
    [Serializable()]
    public class The_Table {
		public string ambiguous_word;				// заполняется в конструкторе из ambiguous_words
		public List<string> senses;				// заполняется в конструкторе из senses_of_ambiguous_words
		public List<string> factors;				// заполняется с помощью Add_factor()
		public float[,] frequencies;				// инициализируется и расчитывается в Compute_frequencies()

		private string[] words_connected_to_senses;	// количество строк соответствует количеству смыслов, "свалка слов", заполняется при анализе корпуса

		// конструктор
		public The_Table(string amb_word, List<string> senses_list)
		{
			this.ambiguous_word = amb_word;
			this.senses = senses_list;
			this.factors = new List<string> ();
			this.words_connected_to_senses = new string[senses_list.Count];
            for (int i = 0; i < this.words_connected_to_senses.Length; i++)
            {
                this.words_connected_to_senses[i] = "";
            }
			//this.frequencies = new float[5, 5];
		}

		// метод, добавляющий новый фактор
		private void Add_factor(string new_factor) {
			this.factors.Add (new_factor);
		}

		// метод, добавляющий слова из предложения в кучу, соответствующую заданному смыслу
		// слова вводятся в словарной форме (основы слов) через пробел
		public void Add_sentence(string in_sentence_sense, string sentence)
		{
			string tmp_sentence = sentence;

			// удаляем многозначное слово из слов предложения
			tmp_sentence = tmp_sentence.Replace(this.ambiguous_word, " ");

			// заменяем возможные двойные пробелы одним пробелом
			while (tmp_sentence.Contains("  "))
			{
				tmp_sentence = tmp_sentence.Replace ("  ", " ");
			}

			// добавляем оставшиеся слова в кучу, соответствующую данному смыслу
			this.words_connected_to_senses[this.senses.IndexOf(in_sentence_sense)] += " " + tmp_sentence;
			this.words_connected_to_senses[this.senses.IndexOf (in_sentence_sense)] = this.words_connected_to_senses[this.senses.IndexOf (in_sentence_sense)].Trim ();
		}

		// метод, расчитывающий частоты
		public void Compute_frequencies()
		{
			// цикл по смыслам
			// {
				// разбить "свалку" во временный массив
				// обработать временный массив
			// }

			// Заполняем factors из массива
			for (int i = 0; i < this.senses.Count; i++)
			{
                if (this.words_connected_to_senses[i].Length != 0)
                {
                    string[] tmp_array = this.words_connected_to_senses[i].Split(' ');

                    for (int j = 0; j < tmp_array.Length; j++)
                    {
                        if (!this.factors.Contains(tmp_array[j]))
                            this.Add_factor(tmp_array[j]);
                    }
                }
			}

			// инициализировать массив частот нулями
			this.frequencies = new float[senses.Count, factors.Count];

            // Посчитать количества связанных со смыслами слов
            for (int i = 0; i < this.senses.Count; i++)
			{
                if (this.words_connected_to_senses[i].Length != 0)
                {
                    string[] tmp_array = this.words_connected_to_senses[i].Split(' ');

                    for (int j = 0; j < tmp_array.Length; j++)
                    {
                        this.frequencies[i, this.factors.IndexOf(tmp_array[j])]++;
                    }
                }
			}

			// Считаем общее количество слов
			int number_of_words = 0;

			for (int i = 0; i < this.senses.Count; i++)
			{
				for (int j = 0; j < this.words_connected_to_senses [i].Length; j++)
					if (this.words_connected_to_senses [i][j] == ' ')
						number_of_words++;

				number_of_words++;
			}
			// считаем частоты

			for (int i = 0; i < this.frequencies.GetLength (0); i++)
				for (int j = 0; j < this.frequencies.GetLength (1); j++)
					this.frequencies [i, j] /= number_of_words;
		}



		// вывод таблицы на экран
		// многозначное слово	f1	f2	f3
		// смысл 1				n1	n1	n1
		// смысл 2				n2	n2	n2
		public void Print_The_Table()
		{
			// вывод первой строки
			Console.Write (this.ambiguous_word + ";");
			foreach (string factor in this.factors)
				Console.Write (factor + ";");

			Console.Write ('\n');

			// вывод остальных строк
			foreach(string sense in this.senses)
			{
				Console.Write(sense + ";");
				for(int j = 0; j<this.frequencies.GetLength(1);j++)
					Console.Write (this.frequencies[this.senses.IndexOf(sense), j] + ";");

				Console.Write ('\n');
			}
		}

	}
}

