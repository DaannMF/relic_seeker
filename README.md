# Parcial 02 - Entrega Final

Fecha de Inicio: 21/05/2025 19:00:00
Fecha de Fin: 08/07/2025 23:59:00

Entrega:
● Descargar Unity Hub y la versión de Unity: 2023.2.20f1 <https://unity.com/es/releases/editor/archive>
● Crear un proyecto 3D.
● Entregar en BlackBoard:
    ○ Link a GDD en Español e Inglés (Opcional).
    ○ Link a itchio.
        ■ La Web debe poseer por lo menos una build Win y WebGL(opcional)
        ■ Link a el repositorio en GitHub.
        ■ Link a GDD en Español e Inglés (Opcional).
    ○ Link a Repositorio.
        ■ Debe tener un ReadMe con:
        ● Link a Itchio.
        ● Link a GDD en Español e Inglés (Opcional).
        ● Detalle del juego. (Qué hacer, Cómo jugar, Autor, Créditos).

Proceso:
    ● Clase a clase se calificará el desarrollo y avances del juego propuestos por el estudiante.
    ● Debe haber un GDD explicando el juego. (Basarse en el Template o el que quieran).
    ● Al final del documento, debe poseer los ‘Sprints‘ semanales que deben ir abarcando cada semana.
    ● Se debe aplicar sin excepción todos los temas vistos en clase, descritos a continuación.

Jugador:
    ● FSM: El jugador, sus interacciones y movimiento debe ser una FSM basada en herencia de States. No
    deben ser Monobehaviour.
    ● Pisadas por Texturas: El jugador debe hacer ruido al moverse (Pisadas, ruido, etc). Éste ruido debe
    ser distinto en por lo menos 2 de estos ejemplos: Pasto, Tierra, Piedra, o algún otro que deseen.
    ● Rampas: El jugador no debe poder subir rampas tanto del terreno como de objetos inclinados. A partir
    de determinado ángulo éste no debe poder avanzar.
    ● Ghost Controller: El jugador debe poder poseer otros objetos. Éste script debe poder ser incluído en
    cualquier modelo o elemento dentro del juego y se debe poder controlar.
    ● Saltos: La cantidad de saltos está limitada en las configuraciones y se debe poder modificar
    fácilmente (Logrando que el jugador pueda saltar por ejemplo 10 veces consecutivas si se desea)
    Entorno:
    ● Escenas Aditivas: El jugador debe permanecer en una escena principal compuesta por un gran
    Terreno. Esta escena nunca debe ser descargada.
        ○ El jugador debe poder entrar a interiores, cargando de manera ‘Async’ y ‘Additive’ las nuevas
        escenas (Sin descargar la principal). Al salir, se debe descargar la escena del interior y volver a
        la principal. En estas transiciones, se debe mostrar ‘Fake Loading Bar’’de por lo menos 1
        segundo.
    ● Terreno: Utilizando Terrain-Tools, el terreno debe poseer alturas, texturas, objetos y pasto.
    ● Fake Load Bar: En las transiciones de escenas, debe haber un FakeLoadingBar.
    ● Audio 3D: En el mundo debe haber Audio Espacial 3D que permita escuchar sonidos proveniente de
        los laterales.

Código y Proyecto:
    ● Los prefabs deben ser autocontenidos. Se debe poder tirar un prefab en la escena y este funciona sin
    la necesidad de que exista otro componente.
    ● Se debe hacer uso correcto de Eventos, Interfaces, herencia y clases abstractas.

UI:
    ● Canvas: Debe visualizar los datos de la partida, por ejemplo: Vida, estamina, nivel actual, progreso,
    enemigos restantes, etc.
    ● Debe haber un Menú Principal.
        ○ Debe haber un panel de créditos.
    ● Debe haber un panel de settings en el que se modifique el audio.
    ● Se debe poder volver al menú principal.
    ● Se debe poder poner pausa.

Para el final

Agregar dificultad para las escenas de interiores
Que el controllable no pueda empujar al player
Win condition
Tercer nivel
Cantidad de Saltos o stamina
Que el player no pueda saltar por el terrain
