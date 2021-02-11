
import StartPage from "./StartPage";
import ReadPage from "./ReadPage";


export type Page = StartPage
    | ReadPage;


export default interface State {
  page: Page;
  user: string;
}
