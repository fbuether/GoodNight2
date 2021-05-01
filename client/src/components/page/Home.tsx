
import {Dispatch} from "../../state/Dispatch";

import {StoryOverview} from "../../state/page/read/StoryOverview";

import Link from "../common/Link";


export interface Home {
  page: "Home"
}

export function Home(state: Home) {
  let storyOverview = Dispatch.Page(StoryOverview.page);

  return (
    <div id="centre" class="row">
      <div class="col-8">
        <h1>Willkommen!</h1>
        <p>
          <em>GoodNight</em> ist eine Platform für interaktive Geschichten –
          Erzählungen, in denen du selbst entscheidest, wie es weitergeht.
          Schlüpfe in die Rolle eines Protagonisten, und tauche auf den Grund
          der Ozeane, erobere die Sterne oder kämpfe um dein Überleben in der
          eiskalten Antarktis.
        </p>
        <h2>Index der Geschichten</h2>
        <p>
          <Link action={storyOverview}>~todo: index~</Link>
        </p>
      </div>
      <div class="col-4">
        <h3>Meine Geschichten</h3>
        <p>Lies hier die Geschichten weiter, die du bereits begonnen hast.</p>
        <p>~todo: liste~</p>
      </div>
    </div>
  );
}
