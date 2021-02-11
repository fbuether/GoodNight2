import type State from "../model/State";
import type {Page} from "../model/Page";


export default class NavigateAction {
  public readonly kind: "navigate" = "navigate";

  public readonly page: Page;

  public constructor(page: Page) {
    this.page = page;
  }

  public execute(state: State) {
    history.pushState(this.page, "GoodNight", this.page.asHref());
    return { ...state, page: this.page };
  }
}
