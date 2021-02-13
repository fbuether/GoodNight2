import type State from "../model/State";
import {asHref, Page} from "../model/Page";


export default function navigate(state: State, page: Page) {
    history.pushState(page, "GoodNight", asHref(page));
    return { ...state, page: page };
}
