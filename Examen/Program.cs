using System;
using System.IO;

string[] lineas = File.ReadAllLines("acb2526.csv"); // se carga el CSV
int opcion = 0;

string[] verEquipos() { // devuelve un array de los 18 equipos 
    string[] equipos = new string[18];
    int total = 0;

    // 1 para saltar la cabecera
    for (int i = 1; i < lineas.Length; i++) {
        string[] campos = lineas[i].Split(",");

        for (int j = 1; j < 3; j++) { // posiciones 1 y 2 (local y visitante) 

            if (Array.IndexOf(equipos, campos[j]) == -1) { // busca el indice en el array, si no esta es -1
                equipos[total] = campos[j];                // por eso cuando no esta, se añade al array
                total++;
            }
        }
    }
    return equipos;
}

string[] resultadosEquipo(string equipo) { // devuelve un array con los resultados de un equipo 
    string[] resultados = new string[34]; // 34 jornadas
    int total = 0;
    for (int i = 1; i < lineas.Length; i++) {
        string[] campos = lineas[i].Split(",");

        if (campos[1] == equipo || campos[2] == equipo) { // si el equipo que se busca es local o visitante se guarda la linea
            resultados[total] = lineas[i];
            total++;
        }
    }
    return resultados;
}

string[] resultadosJornada(string jornada) { // devuelve un array con los resultados de una jornada
    string[] resultados = new string[9]; // 9 partidos por jornada
    int total = 0;
    for (int i = 1; i < lineas.Length; i++) {
        string[] campos = lineas[i].Split(",");

        if (campos[0] == jornada) { // si la jornada que se busca es la misma que la de la linea se guarda la linea
            resultados[total] = lineas[i];
            total++;
        }
    }
    return resultados;
}

string[] top3Mvp() { // devuelve un array con los 3 MVPs que más veces han salido
    
    string[] mvps = new string[lineas.Length]; // la posicion del mvp en este array es la misma
    int[] veces = new int[lineas.Length];      // que la posicion de las veces que ha salido en este otro array
    string[] top = new string[3]; // array final

    // mvp de cada linea
    for (int i = 1; i < lineas.Length; i++) {
        string[] campos = lineas[i].Split(",");
        mvps[i] = campos[4]; // se guarda el mvp de cada linea en el array mvps
    }

    // se coge cada mvp y se compara con todos los demás para contar cuántas veces ha salido
    for (int i = 1; i < lineas.Length; i++) {
        for (int j = 1; j < lineas.Length; j++) {
            if (mvps[i] == mvps[j]) {
                veces[i]++;
            } 
        }
    }

    for (int n = 1; n <= 3; n++) { // se recorre un maximo de 3 mvps
        int mejor = 1;
        for (int i = 1; i < lineas.Length; i++) { 
            if (veces[i] > veces[mejor]) { // se guarda el indice del que mas veces sale
                mejor = i;
            }
        }
        top[n - 1] = $"{n}. {mvps[mejor]} - {veces[mejor]} veces";

        string nombre = mvps[mejor]; // se descartan todas las apariciones de ese jugador
        for (int i = 1; i < lineas.Length; i++) {
            if (mvps[i] == nombre) {
                veces[i] = -1;
            }
        }
    }
    return top;
}

string[] clasificacion() { // devuelve un array con la clasificación de los equipos
    string[] equipos = verEquipos(); // array de los 18 equipos
    int[] victorias = new int[18]; // array de victorias de cada equipo
    int[] partidos = new int[18]; // array de partidos jugados de cada equipo
    int[] puntos = new int[18]; // array de puntos a favor de cada equipo
    string[] tabla = new string[18]; // array final de la clasificación

    for (int i = 0; i < 18; i++) { // se recorre cada equipo
        for (int j = 1; j < lineas.Length; j++) { // se recorre cada linea del CSV
            string[] campos = lineas[j].Split(","); 
            string[] resultado = campos[3].Split("-"); // se separa el resultado en local y visitante
            int ptsLocal = int.Parse(resultado[0]); // se convierten a int los puntos de local y visitante
            int ptsVisitante = int.Parse(resultado[1]);

            if (campos[1] == equipos[i]) { // si el equipo que se busca es local
                partidos[i]++;             // se suma un partido jugado
                puntos[i] += ptsLocal;     // se suman los puntos a favor
                if (ptsLocal > ptsVisitante) { // si gana el partido se suma una victoria
                    victorias[i]++; 
                }
            } else if (campos[2] == equipos[i]) { // si el equipo que se busca es visitante
                partidos[i]++;                    // se suma un partido jugado
                puntos[i] += ptsVisitante;        // se suman los puntos a favor
                if (ptsVisitante > ptsLocal) {    // si gana el partido se suma una victoria
                    victorias[i]++;
                }
            }
        }
    }

    for (int n = 1; n <= 18; n++) { // Empieza en 1 para mostrar la posición en la clasificación
        int mejor = 0; // el indice del mejor equipo es 0 al principio
        for (int i = 0; i < 18; i++) {
            if (victorias[i] > victorias[mejor] || (victorias[i] == victorias[mejor] && puntos[i] > puntos[mejor])) {
                mejor = i; // el que mas victorias tiene es el mejor, si hay empate se mira los puntos a favor
            }
        }
        tabla[n - 1] = $"{n}. {equipos[mejor]} - V: {victorias[mejor]} D: {partidos[mejor] - victorias[mejor]} PF: {puntos[mejor]}";
        victorias[mejor] = -1; // asi se descartan los equipos que ya han salido y no se repiten
    }
    return tabla;
}
  
void guardarEquipos() {
    Directory.CreateDirectory("EQUIPOS"); // crea la carpeta EQUIPOS si no existe
    string[] equipos = verEquipos();

    for (int i = 0; i < equipos.Length; i++) { // guarda un fichero por cada equipo con sus resultados
        File.WriteAllLines("EQUIPOS/" + equipos[i] + ".txt", resultadosEquipo(equipos[i]));
    }

    Console.WriteLine("Ficheros guardados en la carpeta EQUIPOS.");
}

void guardarFicheros(string[] datos) { // guarda los datos en un fichero si el usuario lo desea
    Console.Write("\n¿Guardar en un fichero? (s/n): ");
    string respuesta = Console.ReadLine();

    if (respuesta == "s") {
        Console.Write("Nombre del fichero (sin .txt): ");
        string nombre = Console.ReadLine();
        File.WriteAllLines(nombre + ".txt", datos);
        Console.WriteLine($"Guardado en {nombre}.txt");
    }
}

while (opcion != 6) { // menu principal
    string[] datos = null; // lo que se va a mostrar y guardar 

    Console.WriteLine("\n--- GESTIÓN ACB 25-26 ---");
    Console.WriteLine("1. Resultados de un equipo");
    Console.WriteLine("2. Datos de una jornada");
    Console.WriteLine("3. Top 3 MVPs");
    Console.WriteLine("4. Clasificación");
    Console.WriteLine("5. Guardar un .txt por equipo");
    Console.WriteLine("6. Salir");
    Console.Write("Opción: ");
    int.TryParse(Console.ReadLine(), out opcion);
    Console.WriteLine();

    if (opcion == 1) {
        string[] listaEquipos = verEquipos();
        for (int i = 0; i < listaEquipos.Length; i++) { // muestra la lista de equipos para que el usuario elija
            Console.WriteLine($"{i + 1}. {listaEquipos[i]}");
        }
        Console.Write("Equipo: ");
        string equipo = Console.ReadLine();

        datos = resultadosEquipo(equipo); // carga el array de los datos a mostrar
        if (datos[0] == null) { // si se pone un valor no valido entra aqui
            Console.WriteLine("Error: equipo no encontrado.");
        } else {
            Console.WriteLine($"Resultados de {equipo}:");
            Console.WriteLine("JORNADA, LOCAL, VISITANTE, RESULTADO, MVP, PUNTOS MVP, REBOTES MVP, ASISTENCIAS MVP, EQUIPO MVP");
            for (int i = 0; i < datos.Length; i++) { // muestra los datos
                Console.WriteLine(datos[i]);
            }
            guardarFicheros(datos); // pregunta si se quieren guardar en un fichero
        }
    } else if (opcion == 2) { // lo mismo pero con la jornada en vez de equipo
        Console.Write("Jornada (max: 34): ");
        string jornada = "Jornada " + Console.ReadLine();

        datos = resultadosJornada(jornada);
        if (datos[0] == null) {
            Console.WriteLine("Error: jornada no válida.");
        } else {
            Console.WriteLine($"Resultados de {jornada}:");
            Console.WriteLine("JORNADA, LOCAL, VISITANTE, RESULTADO, MVP, PUNTOS MVP, REBOTES MVP, ASISTENCIAS MVP, EQUIPO MVP");
            for (int i = 0; i < datos.Length; i++) {
                Console.WriteLine(datos[i]);
            }
            guardarFicheros(datos);
        }
    } else if (opcion == 3) {
        Console.WriteLine("TOP 3 MVPs:");
        datos = top3Mvp(); // carga el array del top3
        if (datos != null) {
            for (int i = 0; i < datos.Length; i++) { // se printea si no ha habido ningun problema
                Console.WriteLine(datos[i]);
            }
            guardarFicheros(datos); // se pregunta si se quieren guardar en un fichero
        }
    } else if (opcion == 4) { // lo mismo pero con la tabla en vez del top 3
        Console.WriteLine("CLASIFICACIÓN:");
        datos = clasificacion();
        if (datos != null) {
            for (int i = 0; i < datos.Length; i++) {
                Console.WriteLine(datos[i]);
            }
            guardarFicheros(datos);
        }
    } else if (opcion == 5) {
        guardarEquipos();
    } else if (opcion == 6) {
        Console.WriteLine("Agur!");
    } else {
        Console.WriteLine("Opción no válida");
    }
}