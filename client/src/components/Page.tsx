// import type {State} from "../state/State";

// import {Page as State, Pages} from "../state/Page";

// import {Page as PageModel} from "../state/PageModel";

import {Home} from "./page/Home";
import {SignIn} from "./page/user/SignIn";
import {StoryOverview} from "./page/read/StoryOverview";

import {Navigation} from "./navigation/Navigation";


import {PageState} from "../state/model/PageState";
import {User} from "../state/User";

// import type {Page as PageState} from "../state/Page";

// import HomePage from "./HomePage";
// import ReadPage from "../components/read/ReadPage";
// import WritePage from "../components/write/WritePage";




// function assertNever(param: never): never {
//   throw new Error(`"Page" received invalid page: "${param}"`);
// }

// function getPage(page: PageState) {
//   switch (page.kind) {
//     case "HomePage": return <HomePage {...page} />;
//     case "ReadPage": return <ReadPage {...page} />;
//     case "WritePage": return <WritePage {...page} />;
//     default: return assertNever(page);
//   }
// }


export interface Page {
  page: PageState;
  user: User;
}


function renderPage(page: PageState) {
  switch (page.page) {
    case "Home": return Home(page);
    case "SignIn": return SignIn(page);
    case "StoryOverview": return StoryOverview(page);
    // default: assertNever(page.page);
  }
}



export default function Page(state: Page) {
  return (
    <div id="page"
      class="container-lg shadow-around mt-lg-4 px-2 px-sm-3 px-md-4 pt-lg-1">
        <Navigation page={state.page.page} user={state.user} />
      <hr class="mt-0" />
      {renderPage(state.page)}
    </div>
  );
}
