import Oidc from "oidc-client";


var userManagerConfig = {
  "authority": "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0",
  "client_id": "d8ee60f3-f059-4169-93b4-8faf1c32a9d8",
  "redirect_uri": "http://localhost:32015/finish-sign-in"
};


export interface User {
  level: "guest" | "bearer";
  email?: string;
  name: string;
  authorisation: string;
}


export class UserService {
  private oidc: Oidc.UserManager;

  private guest: null | string;

  private static instance: UserService | null = null;

  private constructor() {
    if (UserService.instance != null) {
      throw new Error("Created second instance of UserService!");
    }

    this.oidc = new Oidc.UserManager(userManagerConfig);

    this.guest = this.loadGuestName();
  }

  public static get(): UserService {
    if (UserService.instance == null) {
      UserService.instance = new UserService();
    }

    return UserService.instance;
  }


  getUser = async() => {
    var user = await this.oidc.getUser();
    if (user !== null) {
      return {
        level: "bearer" as const,
        email: user.profile.email,
        name: user.profile.sub,
        authorisation: "Bearer " + user.id_token
      };
    }

    if (this.guest !== null) {
      return {
        level: "guest" as const,
        name: "Gast",
        authorisation: "Guest " + this.guest
      };
    }

    return null;
  }

  startSignIn = async() => {
    await this.oidc.signinRedirect();
  }

  finishSignIn = async() => {
    try {
      await this.oidc.signinRedirectCallback(window.location.href);
    }
    catch (err) {
      console.log("Error while finishing sign in:", err);
      throw err;
    }
  }


  removeUser = async() => {
    await this.oidc.removeUser();
    window.localStorage.removeItem("guest-id");
  }


  public createNewGuest() {
    var newGuid = this.uuidv4();
    window.localStorage.setItem("guest-id", newGuid);
  }


  private uuidv4() {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g,
      (s: string) => {
        let c = parseInt(s);
        let rnd = crypto.getRandomValues(new Uint8Array(1));
        return (c ^ rnd[0] & 15 >> c / 4).toString(16);
      });
  }

  private loadGuestName(): string | null {
    return window.localStorage.getItem("guest-id");
  }
}
