
import React, { StrictMode, useState, useEffect } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';


import { getArrayObjByID, contactAPI } from "./utilityFuncs";
import { Login, Signup } from "./authFuncs";

import { StoryTab } from "./storyTabComponents";
import { ModalSelectSegmentFromOptionsButton, ModalNewButton } from './storyButtons.jsx';

import { AuthorProfile } from "./authorTabComponents.jsx";
import { AuthorListDisplayButton } from "./authorButtons.jsx";
export default function App() {


    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        contactAPI("load", "get", true)
            .then(function (value) {
                console.log(value);
                setData(value)
            })
        }, []);

    if (!data) {
        return <div>Loading...</div>
    }

    console.log(data);
    if (Object.keys(data).length == 0) {
        console.log("IN HERE")
        switch (authMode) {
            case "login":
                console.log("GONIG TO LOGIN")
                return (
                    <Login
                        onLogin={setData}
                        switchToSignup={() => setAuthMode("signup")}
                    />
                );
                break;
            case "signup":
            default:
                return (
                    <Signup
                        onSignup={setData}
                        switchToLogin={() => setAuthMode("login")}
                    />
                );
                break;
        }
    }

    const rootPath = "/chainmates/";
    const startingWriteOrReview = data.startingUrlDict.readOrWrite;
    let startingStoryID = data.startingUrlDict.storyId;

    let startingURL;
    if (startingWriteOrReview == null) {
        startingURL = "";
    }
    else if (
        (startingWriteOrReview == "write" &&
            getArrayObjByID(data.writeDicts, startingStoryID) == undefined)
        ||
        (startingWriteOrReview == "review" &&
            getArrayObjByID(data.reviewDicts, startingStoryID) == undefined)
    ) {
        startingURL = `${startingWriteOrReview}`;
    }
    else {
        startingURL = `${startingWriteOrReview}/${startingStoryID}`;
        }

    const authorDicts = [{
        "id": data.authorInfo.id,
        "displayName": data.authorInfo.displayName,
        "statsDTO": {
            "writeCount": 0,
            "reviewCount": 0,
            "storyCount":0
        }
        }]
    Object.assign(data, {
        "authorDicts": authorDicts
    });

    async function changeStoryDicts(storyDict, writeOrReview = "write", addOrRemove = "add") {

        let dataKey;
        switch (writeOrReview) {
            case "write":
                dataKey = "writeDicts";
                break;
            case "review":
                dataKey = "reviewDicts";
                break;
            case "author":
                dataKey = "relationInfo";
                break;
        }

        let dictArrayToChange = data[dataKey];
        let newDictArray;
        switch (addOrRemove) {
            case "remove":
                newDictArray = [];
                dictArrayToChange.forEach(originalStoryDict => {
                    if (originalStoryDict.id != storyDict.id) {
                        newDictArray.push(originalStoryDict);
                    }
                }
                )
                break
            case "add":
            default:
                newDictArray = dictArrayToChange.slice();
                newDictArray.push(storyDict);
        }
        if (newDictArray == dictArrayToChange) {
            console.warn("WARNING: ", storyDict, " was not successfully added to / removed from ", dictArrayToChange)
        }
        else { console.log("Successfully changed ", newDictArray, " to ", dictArrayToChange) };

        setData(data => ({ ...data, [dataKey]: newDictArray }));
        return newDictArray;
    }


    return (
            <BrowserRouter>
                <Routes>
                <Route path="" element={<UniversalHeader displayName={data.authorInfo.displayName} />}>
                        <Route path="" relative element={<Home startingURL={startingURL} />}
                            index />
                        <Route path="write/" element={<StoryDashboard writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<StoryTab writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="review/" element={<StoryDashboard writeOrReview="review" dicts={data.reviewDicts}
                            setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<StoryTab writeOrReview="review" dicts={data.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="author/"
                            element={<StoryDashboard writeOrReview="author" dicts={authorDicts} setDicts={changeStoryDicts}
                            />}
                        >
                            <Route path=":tabID/"
                                element={<AuthorProfile writeOrReview="author" dicts={authorDicts} setDicts={changeStoryDicts} />} />
                        </Route>
                    </Route>
                    <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}

function NoMatch() {
    return (
        <div style={{ padding: 20 }}>
            <h2>404: Page Not Found</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adip.</p>
        </div>
    );
}

function Home(props) {

    let navigate = useNavigate();

    useEffect(() => {
        console.log("initital navigate")
        navigate(props.startingURL);
    }, [navigate, props.startingURL])
    return (<div className="container"></div>);
}


function UniversalHeader(props) {

    return (
        <>
            <header className="universalHeader">
                <h1>CHAIN MATES</h1>
                <h1>Hi, {props.displayName}!</h1>
                <nav>
                    <Link to="" ><button type="button">HOME</button></Link>|{" "}
                    <Link to="write" ><button type="button">WRITE</button></Link>|{" "}
                    <Link to="review"><button type="button">READ</button></Link>
                    <Link to="author"><button type="button">AUTHORS</button></Link>
                </nav>
            </header >
            <Outlet></Outlet>
        </>
    )
}



function StoryDashboard(props) {

    console.log(props.dicts)
    let arrayOfTabIDs = props.dicts.map(dict => dict.id);
    const addNewTab = (tabID) => props.setDicts(tabID, props.writeOrReview, "add");
    let getTabName;
    let presavedCurrentContentByStory = {};

    if (props.writeOrReview == "author") {
        console.log(props.dicts)
        getTabName = (id, index) =>
            getArrayObjByID(props.dicts, id).displayName ?? `${index}.`
    }
    else {
        getTabName = (id, index) =>
            getArrayObjByID(props.dicts, id).storyData.title ?? `${index}.`;

        props.dicts.forEach(dictInArray => {
            presavedCurrentContentByStory[dictInArray.id] =
                dictInArray.segmentHistoryList.slice(-1)[0].content;
        })
    }

    const [currentContentByStory, setCurrentContentByStory] = useState(presavedCurrentContentByStory);
    const outlet = useOutlet([currentContentByStory, setCurrentContentByStory]);

    return (
        <div className={props.writeOrReview + "storyDashboardContainer storyDashboardContainer dashboardContainer"}>
            <LeftSidebar writeOrReview={props.writeOrReview} addNewTab={addNewTab} />
            <nav className="tabsList">
                {arrayOfTabIDs.map((tabID, index) =>
                    <Link to={tabID + "/"} key={index + tabID} className="tabLink">
                        <button className="tabButton">{getTabName(tabID, index)}</button>
                    </Link>
                )}
            </nav>
            {outlet || <PlaceHolder />}
        </div >
    )
}

function PlaceHolder() {
    return (<div className="tabContainer">PLACEHOLDER
        <div className="tabContent"></div>
        <div className="footer submissions"></div>
        <div className="rightSidebar comments"></div>
    </div>
    )
}

function LeftSidebar(props) {

    let buttonList;

    switch (props.writeOrReview) {
        case "author":
            buttonList = <AuthorListDisplayButton addAuthorTab={props.addNewTab} />;
            break;
        case "write":
            buttonList = <>
                            <ModalNewButton addNewStory={props.addNewTab} />
                            <ModalSelectSegmentFromOptionsButton type="JOIN" addNewStory={props.addNewTab} />
                        </>
            break;

        case "review":
        default:
            buttonList = <ModalSelectSegmentFromOptionsButton type="MODERATE" addNewStory={props.addNewTab} />
            break;

    }
    return (
        <div className="leftSidebar">
            {buttonList}
        </div>
    )
}

