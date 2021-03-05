import type State from "../model/State";
import type {Page} from "../model/Page";
import DispatchContext from "../DispatchContext";

import Start from "../components/Start";
import Read from "../components/Read";
import Login from "../components/Login";
import Navigation from "../components/Navigation";


function assertNever(param: never): never {
  throw new Error(`"Page" received invalid state: "${param}"`);
}

function getPage(page: Page) {
  switch (page.kind) {
    case "start": return <Start page={page} />;
    case "read": return <Read {...page} />;
    case "login": return <Login />;
    default: return assertNever(page);
  }
}


export default function Page(state: State) {
  return (
    <div id="page"
      class="container-lg shadow mt-md-4 px-2 px-sm-3 px-md-4 py-2 py-md-3">
      <Navigation currentPage={state.page} user={state.user} />
      <hr class="mt-1" />
      {getPage(state.page)}
    </div>
  );
}
