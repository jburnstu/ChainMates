
import React, { useEffect, useState } from "react";
import { BrowserRouter, Link, Outlet, Route, Routes, useNavigate, useOutlet } from 'react-router-dom';

import { initialLoad, Login, Signup } from "./supportFuncs/authFuncs";
import { getArrayObjByID } from "./supportFuncs/utilityFuncs";

import { ModalSelectSegmentFromOptionsButton, StartNewStoryButton } from './buttons/workshopButtons.jsx';
import { WorkshopTab } from "./pages/workshopTab";

import { AuthorSearchButton, StorySearchButton } from "./buttons/searchButtons.jsx";
import { AuthorSearchPage } from "./pages/authorSearchPage";
import { StorySearchPage } from "./pages/storySearchPage";


import { DashboardLayout, PageOrTabLayout } from "./layouts/layouts";


export default function App() {

    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        initialLoad(setData);
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
            case "signup":
            default:
                return (
                    <Signup
                        onSignup={setData}
                        switchToLogin={() => setAuthMode("login")}
                    />
                );
        }
    }

    const rootPath = "/chainmates/";
    const startingWriteOrReview = data.startingUrlDict.readOrWrite;
    let startingStoryID = data.startingUrlDict.storyId;

    let startingURL;
    if (startingWriteOrReview == null) {
        startingURL = "home";
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


    async function changeStoryDicts(storyDict, writeOrReview = "write", addOrRemove = "add") {

        let dataKey;
        switch (writeOrReview) {
            case "write":
                dataKey = "writeDicts";
                break;
            case "review":
                dataKey = "reviewDicts";
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
                        <Route path="" relative element={<RedirectToStartingURL startingURL={startingURL} />}
                            index/>
                        <Route path="home/" relative element={<HomeDashboard authorDict={data.authorInfo} />}
                        />
                        <Route path="write/" element={<WorkshopDashboard writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<WorkshopTab writeOrReview="write" dicts={data.writeDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="review/" element={<WorkshopDashboard writeOrReview="review" dicts={data.reviewDicts}
                            setDicts={changeStoryDicts} />}
                        >
                            <Route path=":tabID/"
                                element={<WorkshopTab writeOrReview="review" dicts={data.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                        </Route>
                        <Route path="authors/" element={<SearchDashboard type="authors" />}>
                            <Route path=":authorID/" element={<AuthorSearchPage self={false} />} />
                        </Route>
                        <Route path="stories/" element={<SearchDashboard type="stories" />}>
                            <Route path=":storyID/" element={<StorySearchPage />} />
                        </Route>
                    </Route>
                    <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}

function UniversalHeader(props) {

    return (
        <div className="container">
            <header className="universalHeader">
                <h1>CHAIN MATES</h1>
                <h1>Hi, {props.displayName}!</h1>
                <nav>
                    <Link to="home" ><button type="button">HOME</button></Link>|{" "}
                    <Link to="write" ><button type="button">WRITE</button></Link>|{" "}
                    <Link to="review"><button type="button">READ</button></Link>
                    <Link to="authors"><button type="button">AUTHORS</button></Link>
                    <Link to="stories"><button type="button">STORIES</button></Link>
                </nav>
            </header >
            <Outlet />
        </div> 
    )
}
function RedirectToStartingURL(props) {

    let navigate = useNavigate();

    useEffect(() => {
        console.log("initital navigate")
        navigate(props.startingURL);
    }, [navigate, props.startingURL])

}

function HomeDashboard(props) {
    return (
         <DashboardLayout 
            leftSidebar={
                null
            }
            tabsList={
                null
            }
            pageOrTab={
                <AuthorSearchPage self={true} authorDict={props.authorDict} />
            }
        />
    )
}

function WorkshopDashboard(props) {
    console.log(props.writeOrReview, props.dicts)
    let arrayOfTabIDs = props.dicts.map(dict => dict.id);
    const addNewTab = (tabID) => props.setDicts(tabID, props.writeOrReview, "add");

    const getTabName = (id, index) =>
        getArrayObjByID(props.dicts, id).storyData.title ?? `${index}.`;

    let presavedCurrentContentByStory = {};
    props.dicts.forEach(dictInArray => {
        presavedCurrentContentByStory[dictInArray.id] =
            dictInArray.segmentHistoryList.slice(-1)[0].content;
    })

    const [currentContentByStory, setCurrentContentByStory] = useState(presavedCurrentContentByStory);
    const outlet = useOutlet([currentContentByStory, setCurrentContentByStory]);

    return (
        <DashboardLayout 
            leftSidebar={
                (props.writeOrReview == "write")
                    ?
                    <>
                        <StartNewStoryButton addNewStory={addNewTab} />
                        <ModalSelectSegmentFromOptionsButton writeOrReview={props.writeOrReview} addNewStory={addNewTab} />
                    </>
                    :
                    <ModalSelectSegmentFromOptionsButton writeOrReview={props.writeOrReview} addNewStory={addNewTab} />
            }
            tabsList={
                arrayOfTabIDs.map((tabID, index) =>
                    <Link to={tabID + "/"} key={index + tabID} className="tabLink">
                        <button className="tabButton">{getTabName(tabID, index)}
                        </button>
                    </Link>
                )
            }
            pageOrTab={
                outlet || <PageOrTabLayout/>
            }
        />
    )
}

function SearchDashboard(props) {
    const outlet = useOutlet();
    return (
         <DashboardLayout 
            leftSidebar={
                (props.type == "authors") 
                    ?
                    <AuthorSearchButton />
                    :
                    <StorySearchButton />
            }
            tabsList={
                null
            }
            pageOrTab={
                outlet || <PageOrTabLayout/>
            }
        />
    )
}

function NoMatch() {
    return (
        <div style={{ padding: 20 }}>
            <h2>404: Page Not Found</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adip.</p>
        </div>
    );
}


//function Header() {
//    const navigate = useNavigate();

//    function handleLogout() {
//        // 1. Clear auth data
//        localStorage.removeItem("token");

//        // 2. (optional) clear anything else
//        // sessionStorage.clear();

//        // 3. Redirect
//        navigate("/login");
//    }

//    return (
//        <div className="header">
//            <h1>My App</h1>
//            <button onClick={handleLogout}>Logout</button>
//        </div>
//    );
//}