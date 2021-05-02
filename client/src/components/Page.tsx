
import {Home} from "../pages/Home";
import {SignIn} from "../pages/user/SignIn";
import {StoryOverview} from "../pages/read/StoryOverview";
import {ReadStory} from "../pages/read/ReadStory";

import {Navigation} from "./navigation/Navigation";


import type {Pages} from "../state/Pages";
import {User} from "../state/User";


export interface Page {
  page: Pages;
  user: User;
}


function assertNever(param: never): never {
  throw new Error(`Invalid page kind in Page.renderPage: "${param}"`);
}

function renderPage(page: Pages) {
  switch (page.page) {
    case "Home": return Home(page);
    case "SignIn": return SignIn(page);
    case "StoryOverview": return StoryOverview(page);
    case "ReadStory": return ReadStory(page);
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
