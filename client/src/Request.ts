

// serviceBase is defined in index.html
declare var serviceBase: string;


type Method = "GET" | "POST";


export default async function request(method: Method, url: string,
  body: any = {}) : Promise<object> {

  let fetchInit: { method: string, body?: string } = {
    method: method
  };

  if (method != "GET" && Object.keys(body).length > 0) {
    fetchInit.body = JSON.stringify(body);
  }

  let fullUrl = serviceBase + (url.startsWith("/") ? url : "/" + url);
  let request = await fetch(fullUrl, fetchInit);
  let json = await request.json();
  return json;
}
