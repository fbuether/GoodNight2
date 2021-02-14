
import type {StartPage} from "../model/Page";


export default function Start(state: { page: StartPage }) {
  return (
    <div id="centre">
      <h1>Willkommen!</h1>
      <p>
        <em>GoodNight</em> ist eine Platform für interaktive Geschichten –
        Erzählungen, in denen du selbst entscheidest, wie es weitergeht.
        Schlüpfe in die Rolle eines Protagonisten, und tauche auf den Grund der
        Ozeane, erobere die Sterne oder kämpfe um dein Überleben in der
        eiskalten Antarktis.
      </p>
      <h2>Index der Geschichten</h2>
    </div>
  );
}
