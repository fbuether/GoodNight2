import Scene from "../play/Scene";
import State from "../play/State";
import Log from "../play/Log";


export default interface Frame {
  game: string;
  log: Log;
  scene: Scene;
  state: State;
}


export default function Frame() {
  let frame: Frame = {
    game: "Holms Schlucht",
    log: {
      entries: [
        {
          type: "story",
          story: {
            name: "first story",
            text: `
In der Schmiede steht eine hünenhafte Frau, die mit einem gewaltigen Hammer auf ein Eisen einschlägt. Der Geruch von Hitze und Asche liegt in der Luft.
`
          }
        },
        {
          type: "story",
          story: {
            name: "second story",
            text: `
Auf der Werkbank liegen verschiedenste Eisenteile verteilt: Spitzhacken, Türbeschläge, einfache Nägel, ein riesiges Zahnrad. An der gegenüberliegenden Wand hängt das Werkzeug der Schmiedin.

Die Schmiedin wirft das gehämmerte Eisenteil zurück in die Esse und wendet sich zur dir um. "Ja?"
`
          }
        },
        {
          type: "action",
          action: {
            name: "the action",
            text: `
"Was schmiedest du hier?"
`
          }
        }
      ]
    },
    scene: {
      name: "schmiedeantwort1",
      text: `"Alles, nur keine Waffen."
Sie wirft ihren massigen Hammer auf die Werkbank, und setzt sich auf einen kleinen Schemel. Die Muskeln ihrer Oberarme vollführen bei der kleinen Bewegung einen beeindruckenden Tanz.

"Wenn du irgendetwas aus Eisen brauchst, bist du hier richtig. Alltagsgegenstände, Baumaterial, Ausrüstung für Abenteurer. Beschläge für deine Rüstung, vielleicht? Ich kann dir auch Hufeisen machen, ich hab nur noch nie ein Pferd hier unten gesehen."
`,
      choices: [
        {
          name: "ruestungkaufen",
          text: `"Ich brauche eine Rüstung."`,
          available: false,
          requirements: [
            {
              name: "Münzen",
              icon: "two-coins",
              relation: "mindestens",
              required: "40",
              has: "2"
            }
          ]
        },
        {
          name: "frage1",
          text: `"Vermisst du die Oberfläche?"`,
          available: true,
          requirements: []
        },
        {
          name: "frame2",
          text: `"Ich habe von einem seltsamen Material gehört, das in den tiefen Gängen abgebaut worden sein soll."`,
          available: true,
          requirements: [
            {
              name: "Das Unobtanium",
              icon: "shamrock",
              required: "2",
              relation: "genau",
              has: "2"
            },
            {
              name: "Ärger mit Fina",
              icon: "two-coins",
              required: "no",
              relation: "genau",
              has: "no"
            }
          ]
        }
      ]
    },
    state: {
      name: "Perrywinkle",
      qualities: [
        {
          name: "Münzen",
          icon: "two-coins",
          type: "int",
          has: "2"
        },
        {
          name: "Glückspilz",
          icon: "shamrock",
          type: "bool",
          has: "yes"
        },
        {
          name: "Ärger mit Fina",
          icon: "shamrock",
          type: "enum",
          has: "Fina hat Deine Geschichte gehört, und vertraut Dir… vorerst."
        }
      ]
    }
  };

  return (
    <div id="centre" class="row px-0 g-0">
      <div id="text" class="col-sm-8">
        <h1 id="banner">{frame.game}</h1>
        <Log {...frame.log}></Log>
        <Scene {...frame.scene}></Scene>
      </div>
      <div id="side" class="col-sm-4">
        <hr class="w-75 mx-auto mt-4 mb-5" />
        <State {...frame.state}></State>
      </div>
    </div>
  );
}

