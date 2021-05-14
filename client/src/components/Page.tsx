
import {Home} from "../pages/Home";
import {SignIn} from "../pages/user/SignIn";
import {StoryOverview} from "../pages/read/StoryOverview";
import {ReadStory} from "../pages/read/ReadStory";
import {SelectStory} from "../pages/write/SelectStory";
import {CreateStory} from "../pages/write/CreateStory";
import {WriteStory} from "../pages/write/WriteStory";
import {WriteScene} from "../pages/write/WriteScene";
import {WriteQuality} from "../pages/write/WriteQuality";
import {StartAdventure} from "../pages/read/StartAdventure";

import {Navigation} from "./navigation/Navigation";


import type {Pages} from "../state/Pages";
import {User} from "../state/User";


export interface Page {
  page: Pages;
  user: User;
}


function renderPage(page: Pages) {
  switch (page.page) {
    case "Home": return Home(page);
    case "SignIn": return SignIn(page);
    case "StoryOverview": return StoryOverview(page);
    case "ReadStory": return ReadStory(page);
    case "SelectStory": return SelectStory(page);
    case "CreateStory": return CreateStory(page);
    case "WriteStory": return WriteStory(page);
    case "WriteScene": return WriteScene(page);
    case "WriteQuality": return WriteQuality(page);
    case "StartAdventure": return StartAdventure(page);
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
