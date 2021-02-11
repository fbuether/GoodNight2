import State, {Page} from "../model/State";
import DispatchContext from "../DispatchContext";

import Frame from "./play/Frame";
import Start from "./pages/Start";
import Read from "./pages/Read";

import Navigation from "./Navigation";


function getPage(page: Page) {
  switch (page.kind) {
      case "start": return <Start {...page}></Start>;
      case "read": return <Read {...page}></Read>;

      default: let exhaust: never = page; return exhaust;
  }
}


export default function Page(state: State) {
  return (
    <div id="page"
      className="container-lg shadow mt-md-4 px-2 px-sm-3 px-md-4 py-2 py-md-3">
      <Navigation />
      <hr className="mt-1" />
      {getPage(state.page)}
    </div>
  );
}
