using System.Collections.Generic;
using UnityEngine;
public static class LenguageSplit
{
    public static Dictionary<SystemLanguage, Dictionary<string, string>> LoadCSV(string sheet)
    {
        var codex = new Dictionary<SystemLanguage, Dictionary<string, string>>();
        var langColumn = new Dictionary<int, SystemLanguage>();
        var idColumn = 0;
        // se divide el texto en lineas
        var lines = sheet.Split(new[] { '\n' , '\r' },System.StringSplitOptions.RemoveEmptyEntries);
        bool isFirstLine = true; // se utiliza para identificar la primera linea que contiene los nombres de las columnas
        foreach (var line in lines) // se itera sobre cada linea del texto
        {
            var cells = line.Split(','); // se divide cada linea en celdas utilizando la coma como separador porque lo guarde como CSV la planilla
            if (isFirstLine)//si es la primera linea en mi caso ID,Spanish,English
            {

                isFirstLine = false; // se marca que ya se ha procesado la primera linea
                for (int i = 0; i < cells.Length; i++)
                {
                    if (!cells[i].Contains("ID")) //si no coniente la palabra ID se asume que es una columna de idioma y se intenta parsear el nombre del idioma a un valor de SystemLanguage
                    {
                        try
                        {
                            //Parse = convertir una cadena de texto (string) en otro tipo de datos especifico en este caso un SystemLanguage
                            langColumn[i] = (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), cells[i]);
                            //para ese indice de la iteracion de la celda se guarda el valor del idioma parseado en el diccionario langColumn
                            //en mi caso Spanish[1] = SystemLanguage.Spanish, English[2] = SystemLanguage.English
                        }
                        catch
                        {
                            continue;
                        }
                        if (!codex.ContainsKey(langColumn[i]))//aca se comineza a llenar el codex con los idiomas encontrados en la primera linea, se crea una nueva entrada para cada idioma con un diccionario vacio para almacenar las traducciones
                        {
                            codex[langColumn[i]] = new Dictionary<string, string>();
                            // codex =
                            //{
                            // Spanish : {}
                            // English : {}
                            //}
                        }
                    }
                    else
                    {
                        idColumn = i; // se guarda el indice donde se encontro la columna ID (en mi CSV es 0)
                    }
                }
                continue;
            }
            for (int i = 0; i < cells.Length; i++)
            {
                // se saltea la columna ID porque contiene la clave de traduccion
                // y no un texto traducido
                if (i == idColumn)
                {
                    continue;
                }
                // verifica que esta columna corresponda a un idioma detectado
                // en la primera linea del CSV
                if (!langColumn.ContainsKey(i))
                {
                    continue;
                }
                // obtiene el idioma asociado a esta columna
                SystemLanguage lang = langColumn[i];
                // obtiene el ID desde la columna marcada como ID
                // (en mi CSV idColumn vale 0)
                string id = cells[idColumn];
                // obtiene el texto traducido de esta columna
                string textValue = cells[i];
                // guarda:
                // idioma -> (ID -> texto traducido)
                codex[lang][id] = textValue;
            }
        }
        return codex;
    }
}
