

// serviceBase is defined in index.html
declare var serviceBase: string;


type Method = "GET" | "POST";

interface FetchInit {
  method: string;
  body?: string;
  headers?: Headers;
}


export default async function request(method: Method, url: string,
  body: any = {}) : Promise<object> {

  let fetchInit: FetchInit = {
    method: method
  };

  if (body != {}) {
    fetchInit.headers = new Headers();
    fetchInit.headers.append("Content-Type", "application/json");
  }

  if (method != "GET" && Object.keys(body).length > 0) {
    fetchInit.body = JSON.stringify(body);
  }

  let fullUrl = serviceBase + (url.startsWith("/") ? url : "/" + url);
  let request = await fetch(fullUrl, fetchInit);
  let json = await request.json();
  return json;
}
