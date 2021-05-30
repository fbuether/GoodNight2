import {Home} from "../pages/Home";
import {FinishSignIn} from "../pages/user/FinishSignIn";
import {RequireSignIn} from "../pages/user/RequireSignIn";
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


const othftwy = require("../../assets/othftwy.gif");


export interface Page {
  page: Pages;
  user: User;
}


function renderPage(page: Pages) {
  switch (page.page) {
    case "ReadStory": return ReadStory(page);
    case "StartAdventure": return StartAdventure(page);
    case "StoryOverview": return StoryOverview(page);

    case "FinishSignIn": return FinishSignIn(page);
    case "RequireSignIn": return RequireSignIn(page);

    case "CreateStory": return CreateStory(page);
    case "SelectStory": return SelectStory(page);
    case "WriteQuality": return WriteQuality(page);
    case "WriteScene": return WriteScene(page);
    case "WriteStory": return WriteStory(page);

    case "Home": return Home(page);
  }
}


export default function Page(state: Page) {
  return (
    <div id="page"
      class="container-lg shadow-around mt-lg-4 px-2 px-sm-3 px-md-4 pt-lg-1">
        <Navigation page={state.page.page} user={state.user} />
      <hr class="mt-0" />
      {renderPage(state.page)}
      <footer id="schlussvermerk">
        <hr class="decorated w-75" />
        GoodNight 2 ~{" "}
        <a href="https://jasminefields.net/" title="jasminefields.net">jasminefields.net</a> ~{" "}
        <a href="https://discord.gg/qwrzStrDwA" target="_blank">discord</a> ~{" "}
        <a href="https://github.com/fbuether/GoodNight2">github</a> ~{" "}
        <img src={othftwy} title="on the hunt for the white yonder" alt="hunt" />
      </footer>
    </div>
  );
}
