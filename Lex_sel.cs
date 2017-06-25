using System;
using System.Collections.Generic;

namespace Test_lec_sel
{
	public class Lex_sel
	{
		//Ссылка на куб
		//TODO заменить чтением из файла
		List<The_Table> cube;

		public Lex_sel (List<The_Table> cub)
		{
			this.cube = cub;
		}

		//поиск смысла многозначного слова
		public string select_sence (string word_to_translate, string[] context_words)
		{
			// находим слово в кубе
			bool found = false;
			int index_in_cube = 0;

			foreach (The_Table table in this.cube)
			{
				if (table.ambiguous_word == word_to_translate)
				{
					found = true;
					index_in_cube = this.cube.IndexOf (table);
					break;
				}
			}

			// если слова в кубе нет - возвращаем пустую строку 
			if (found == false)
			{
				//Console.WriteLine ("Translation not found");
				return "";
			}

				
			// создаём int массив размером с количество смыслов

			float[] sence_scores = new float[this.cube[index_in_cube].senses.Count];
			int factor_index;

			// в циклах проверяем каждое слово из контекста по смыслам
			for(int i = 0; i<context_words.Length; i++)
				if (this.cube[index_in_cube].factors.Contains(context_words[i]))
				{
					factor_index = this.cube [index_in_cube].factors.IndexOf (context_words [i]);
					for (int j = 0; j < sence_scores.Length; j++)
						sence_scores [j] += this.cube [index_in_cube].frequencies [j, factor_index];
				}
					
			// выбираем смысл с максимальным баллом
			float max_score = 0;
			int index_of_max_score = 0;

			for (int i = 0; i < sence_scores.Length; i++)
			{
				if (sence_scores [i] > max_score)
				{
					max_score = sence_scores [i];
					index_of_max_score = i;
				}
			}

			return this.cube [index_in_cube].senses [index_of_max_score];
		}
	}
}