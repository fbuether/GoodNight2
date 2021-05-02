import type {Dispatch} from "./Dispatch";
import type {PageDescriptor} from "./PageDescriptor";

// import type {Page} from "../state/Page";


export const History = {
  register: (gotoUrl: (url: string) => void) => {
    window.addEventListener("popstate", (event: PopStateEvent) => {
      let restoredUrl = event.state;
      gotoUrl(restoredUrl);
      // let restoredPage = Page.ofUrl(restoredUrl);
      // sender(Dispatch.Page(restoredPage));
    });
  },


  push: (desc: PageDescriptor) => {
    if (history.state != desc.url) {
      history.pushState(desc.url, desc.title, desc.url);
    }
  }
}
