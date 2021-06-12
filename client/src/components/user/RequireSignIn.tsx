import type {RequireSignIn as State} from "../../state/page/user/RequireSignIn";

import Link from "../../components/common/Link";
import Icon from "../../components/common/Icon";


export function RequireSignIn(state: State) {
  return (
    <div id="centre" class="row">
      <h3>Anmeldung notwendig</h3>
      <p>
        Um fortzufahren, musst du dich bei GoodNight anmelden. Bitte wähle aus,
        auf welche Weise du dich anmelden willst.
      </p>
      <div class="row gy-3">
        <div class="col-md-6">
        <Link class="boxed text-start px-4 pt-3 pb-2 h-100" action={state.signInGuest}>
          <h4>
            <Icon name="ninja-head" class="higher mr-2" />
            Als Gast fortfahren
          </h4>
          <p>
            Als Gast kannst du Geschichten lesen, und dein Spielstand wird
            in diesem Browser gespeichert. Du kannst also später weiterlesen.
          </p>
          <p>
            <strong>Achtung:</strong>
            Wenn du allerdings die Daten deines Browsers löscht, ist dein
            Spielstand unwiederbringlich verloren.
          </p>
          <p>
            Ebenfalls kannst du als Gast keine neuen Geschichten schreiben.
          </p>
        </Link>
    </div>
    <div class="col-md-6">
        <Link class="col boxed text-start px-4 pt-3 pb-2 h-100" action={state.signInUser}>
          <h4>
            <Icon name="astronaut-helmet" class="higher mr-2" />
            Anmelden
          </h4>
          <p>
            Du kannst dich bei GoodNight anmelden, um auch langfristig
            auf deine Spielstände zuzugreifen.
          </p>
          <p>
            GoodNight nutzt dafür die Anmeldung bei Microsoft, also kannst du
            dich z.B. mit deinem Windows-Account anmelden. Das schützt deine
            Daten, da GoodNight dein Passwort niemals sehen kann.
          </p>
          <p>
            Mit einer vollen Anmeldung kannst du ebenfalls selbst Geschichten
            schreiben, die andere lesen können.
          </p>
        </Link>
    </div>
      </div>
    </div>
  );
}
