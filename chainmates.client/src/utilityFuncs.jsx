//import { useContext } from "react";
import { AuthorContext} from "./context.jsx";
//import { useNavigate } from "react-router-dom";

export default { getRandomItem, contactAPI, getArrayObjByID }



export async function contactAPI(urlTarget, method, authorized = false, bodyDict = {}) {

    const urlStub = "/chainmates/"
    let credentials = authorized ? "include" : "same-origin";
    let fetchData;
    switch (method) {
        case "get":
            fetchData = {
                method: method,
                credentials: credentials
            }
            break;
        default:
            fetchData = {
                method: method,
                credentials: credentials,
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(bodyDict)
            }
    }
    console.log(bodyDict);

    let response = await fetch(`${urlStub}${urlTarget}`,
        fetchData
    )

    if ((method == "get" && response.status != 200) || !response.ok) {
        console.log("HTTP Error ", response.status, "at url ", `${urlStub}${urlTarget}`);
        return {};
    }
    else {
        console.log("SUCCESS at url ", `${urlStub}${urlTarget}`);
    }
    //console.log(response);
    return response.json();
};


export function getArrayObjByID(array, id) {
    const idMatch = (obj) => obj.id == id;
    return array.find(idMatch);
}



export function getRandomItem(array, numberOfResults = 1, arrayOfOne = false) {
    console.log("started getRandomItem");
    if ((numberOfResults == 1 || array.length == 1) && !(arrayOfOne)) {
        return array[Math.floor(Math.random() * array.length)];
    }

    let set = new Set();
    while (set.size < numberOfResults && set.size < array.length) {
        console.log("Inside the while");
        var randomIndex = Math.floor(Math.random() * array.length);
        set.add(randomIndex);
    }
    let randomIndexArray = Array.from(set);
    console.log("finished getrandomitem");

    return randomIndexArray.map(index => array[index]);
}