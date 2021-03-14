
import type {State} from "../state/State";
import type {Page as PageState} from "../state/Page";
import DispatchContext from "../DispatchContext";

import HomePage from "./HomePage";
import ReadPage from "../components/read/ReadPage";
import WritePage from "../components/write/WritePage";
import Navigation from "../components/Navigation";


function assertNever(param: never): never {
  throw new Error(`"Page" received invalid state: "${param}"`);
}

function getPage(page: PageState) {
  switch (page.kind) {
    case "HomePage": return <HomePage {...page} />;
    case "ReadPage": return <ReadPage {...page} />;
    case "WritePage": return <WritePage {...page} />;
    default: return assertNever(page);
  }
}


export default function Page(state: State) {
  return (
    <div id="page"
      class="container-lg shadow-around mt-lg-4 px-2 px-sm-3 px-md-4 pt-lg-1">
      <Navigation currentPage={state.page} user={state.user} state={state} />
      <hr class="mt-0" />
      {getPage(state.page)}
    </div>
  );
}
