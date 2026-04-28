
import React, { useEffect, useState } from "react";
import { BrowserRouter, Link, Outlet, Route, Routes, useNavigate, useLocation, useOutlet, Navigate } from 'react-router-dom';

//import { initialLoad, Login, Signup } from "./supportFuncs/authFuncs";
import { ModalSelectSegmentFromOptionsButton, StartNewStoryButton } from './buttons/workshopButtons.jsx';
import { WorkshopTab } from "./pages/workshopTab";

import { AuthorSearchButton, StorySearchButton } from "./buttons/searchButtons.jsx";
import { AuthorSearchPage } from "./pages/authorSearchPage";
import { StorySearchPage } from "./pages/storySearchPage";
import { Login, Signup } from "./pages/loginAndSignupPages";
import { SettingsPage } from "./pages/settingsPage";

import { contactAPI, getArrayObjByID } from "./utilityFuncs.jsx";

import { DashboardLayout, PageOrTabLayout } from "./layouts/layouts";

export default function App() {

    ///////  Load initial data or shunt into login/signup facade ////////////
    const [user, setUser] = useState(null);
    const [data, setData] = useState(null);
    const [authMode, setAuthMode] = useState("login"); 

    useEffect(() => {
        console.log("IN APP USEEFFECT");
        const initialLoad = async () => {
            let initialLoadData = await contactAPI("load", "get", true);
            setData(initialLoadData);
            setUser(initialLoadData.authorInfo);
        }
        if (data === null) {
            initialLoad();
        }

        });

    if (user === null) {
        return <div>Loading...</div>
    }

    async function handleLogout() {
        console.log("in app handle logout");
        await contactAPI("auth/logout/", "post", true);

        setUser(null);
        setData(null);
    }


    ///////////// The function used to add / remove tabs when stories are loaded / submitted /////////////
    //////////// Passed down to various components (could be context perhaps?)             ///////////////

    async function changeStoryDicts(storyDict, writeOrReview = "write", addOrRemove = "add") {

        let dictArrayToChange = data[`${writeOrReview}Dicts`];
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

        setData(data => ({ ...data, [`${writeOrReview}Dicts`]: newDictArray }));
        return newDictArray;
    }

    const onLogin = (initialData) => {
        setData(initialData);
        setUser(initialData.authorInfo);
    }

    return ( //Could be a separate browserroutes document at some point
            <BrowserRouter>
                <Routes>
                <Route path="" element={<UniversalHeader displayName={user?.displayName} handleLogout={handleLogout} />} >
                    <Route index path="login/" element={<Login onLogin={onLogin}
                                                        />} />
                    <Route path="signup/" element={<Signup onLogin={onLogin}
                                    />} />
                    <Route path="home/" element={
                        <ProtectedRoute user={user} children={
                            <HomeDashboard authorDict={data?.authorInfo} />
                        } />} >
                    </Route>
                    <Route path="write/" element={
                        <ProtectedRoute user={user} children={
                            <WorkshopDashboard writeOrReview="write" dicts={data?.writeDicts} setDicts={changeStoryDicts} />
                        } />} >
                        <Route path=":tabID/"
                            element={<WorkshopTab writeOrReview="write" dicts={data?.writeDicts} setDicts={changeStoryDicts} />}
                            />
                    </Route>
                    <Route path="review/" element={
                        <ProtectedRoute user={user} children={
                            <WorkshopDashboard writeOrReview="review" dicts={data?.reviewDicts} setDicts={changeStoryDicts} />
                        } />} >
                        <Route path=":tabID/"
                            element={<WorkshopTab writeOrReview="review" dicts={data?.reviewDicts} setDicts={changeStoryDicts} />}
                            />
                    </Route>
                    <Route path="authors/" element={
                        <OptionalAuthRoute user={user} children={
                            <SearchDashboard type="authors" />
                        } />} >
                        <Route path=":authorID/"
                            element={<AuthorSearchPage />}
                        />
                    </Route>
                    <Route path="stories/" element={
                        <OptionalAuthRoute user={user} children={
                            <SearchDashboard type="stories" />
                        } />} >
                        <Route path=":storyID/"
                            element={<StorySearchPage />}
                        />
                    </Route>

                    <Route path="settings/" element={
                        <ProtectedRoute user={user} children={
                            <SettingsPage authorInfo={data?.authorInfo} handleLogout={handleLogout} />
                        } />} >
                    </Route>

                    </Route>
                <Route path="*" element={<NoMatch />} />
                </Routes>
            </BrowserRouter>
    );
}


function ProtectedRoute({ user, children }) {
    const location = useLocation();
    //const navigate = useNavigate();

    console.log("IN PROTECTED ROUTE CHECK" ,user);

    if (user === null) return <div>Loading...</div>;

    if (!user) {
        console.log("in !user branch")
        //navigate("/login");
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return children;
}

function OptionalAuthRoute({ user, children }) {
    console.log("IN OPTIONAL ROUTE CHECK", user);
    if (user === null) return <div>Loading...</div>;
    return children;
}
function UniversalHeader(props) {
    const navigate = useNavigate();

    const handleLogout = () => {
        props.handleLogout();
        navigate("/login");
    }

    console.log("IN UNIVERSAL HEADER",props.displayName);
    return (
        <div className="container">
            <header className="universalHeader">
                <h1>CHAIN MATES</h1>
                <h1>Hi, {props.displayName}!</h1>
                <nav>

                    <Link to="home" ><button type="button">HOME</button></Link>|{" "}
                    <Link to="write" ><button type="button">WRITE</button></Link>|{" "}
                    <Link to="review"><button type="button">READ</button></Link>|{" "}
                    <Link to="authors"><button type="button">AUTHORS</button></Link>|{" "}
                    <Link to="stories"><button type="button">STORIES</button></Link>|{" "}
                    <Link to="settings"><button type="button">SETTINGS</button></Link>|{" "}
                    {props.displayName
                        ? <button type="button" onClick={handleLogout}>LOG OUT</button>
                        : <Link to="login"><button type="button">LOG IN</button></Link>       
                    }

                </nav>
            </header >
            <Outlet />  {/*The rest of the app */}
        </div> 
    )
}

function HomeDashboard(props) {
    console.log("HOME");
    // Loads up your own AuthorDashboard (same as everyone else with a couple tweaks)
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
    console.log(`IN ${props.writeOrReview} dashboard`);

    let arrayOfTabIDs = props.dicts.map(dict => dict.id);

    const addNewTab = (tabID) => props.setDicts(tabID, props.writeOrReview, "add");
    const getTabName = (tabID, index) =>
        getArrayObjByID(props.dicts, tabID).storyData.title ?? `${index}.`;


    // Assign every outlet option its own "current content", so they can be saved in parallel
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
                (props.writeOrReview == "write") //better as switch case?
                    ?
                    <>
                        <StartNewStoryButton addNewStory={addNewTab} />
                        <ModalSelectSegmentFromOptionsButton type="JOIN" addNewStory={addNewTab} />
                    </>
                    :
                        <ModalSelectSegmentFromOptionsButton type="MODERATE" addNewStory={addNewTab} />
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
    console.log(`in ${props.type} search page`)
    const outlet = useOutlet();
    return (
         <DashboardLayout 
            leftSidebar={
                (props.type == "authors") // switch case?
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
