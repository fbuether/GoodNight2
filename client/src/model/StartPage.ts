
export default class StartPage {
  public kind: "start" = "start";

  public message: string;

  public constructor(msg: string) {
    this.message = msg;
  }

  public asHref() {
    return "/start";
  }
};
