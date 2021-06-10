import {UserService} from "./UserService";


export type Method = "GET" | "POST" | "PUT" | "DELETE";

interface FetchInit {
  method: string;
  body?: string;
  headers?: Headers;
}

export interface ErrorResponse {
  kind: "error";
  status: number;
  isError: true;
  isResult: false;
  type: string;
  message: string;
}

export interface ResultResponse<T> {
  kind: "result";
  isError: false;
  isResult: true;
  message: T;
}

export type Response<T> = ErrorResponse | ResultResponse<T>;

export function isResult<T>(a: undefined | null | Response<T>)
: a is ResultResponse<T> {
  return a !== undefined && a !== null && typeof a == "object" && a.isResult;
}



export function makeResult<T>(body: unknown): ResultResponse<T> {
  return {
    kind: "result",
    isError: false,
    isResult: true,
    message: body as T
  };
}

export function makeError(status: number, body: any): ErrorResponse {
  let errorType = "type" in body ? body.type : "no error type on response.";
  let message = "message" in body ? body.message : "no message on response.";

  return {
    kind: "error",
    status: status,
    isError: true,
    isResult: false,
    type: errorType,
    message: message
  };
}


// define url to service, which resides at the same domain as us.
const port = window.location.port == "32015"
    ? ":32016"
    : window.location.port != ""
    ? ":" + window.location.port
    : "";
const serviceBase = window.location.protocol + "//" + window.location.hostname +
    port;


export async function request<T>(method: Method, url: string,
  body: any = {}) : Promise<Response<T>> {

  let fetchInit: FetchInit = {
    method: method
  };

  fetchInit.headers = new Headers();
  if (body != {}) {
    fetchInit.headers.append("Content-Type", "application/json");
  }

  let user = await UserService.get().getUser();
  if (user !== null) {
    fetchInit.headers.append("Authorization", user.authorisation);
  }

  if (method != "GET" && Object.keys(body).length > 0) {
    fetchInit.body = JSON.stringify(body);
  }

  try {
    let fullUrl = serviceBase + (url.startsWith("/") ? url : "/" + url);
    let response = await fetch(fullUrl, fetchInit);
    let body = method == "DELETE" ? await response.text()
        : await response.json();

    if (response.ok) {
      return makeResult<T>(body);
    }
    else {
      console.warn("Returned invalid result", response.status, body);
      return makeError(response.status, body);
    }
  }
  catch (err) {
    console.warn("Exception during request", err);
    return makeError(0, { type: "Exception", message: String(err) });
  }
}
