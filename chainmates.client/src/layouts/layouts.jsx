


/// These layouts are used to standardise the "look" of the app (the classNames here   //////
/// bear the brunt of the structural CSS used). PageOrTabLayout is the outlet of    /////////
//// DashboardLayout. Adpoting these layouts has resulted in a bit of a div explosion --   //
/// for now, I've just called the classes inner and outer so i at least know which //////////
///// one i'm working with.   ///////////////////////////////////////////////////////////////
export function DashboardLayout({ leftSidebar, tabsList, pageOrTab }) {
    return (
        <div className="dashboardContainer">
            <div className="leftSidebar">{leftSidebar}</div>
            <div className="tabsList">{tabsList}</div>
            <div className="pageOrTabOuterContainer">{pageOrTab}</div>
        </div>
    )
}

export function PageOrTabLayout({ topLine, mainContent, footer, rightSidebar }) {
    return (
        <div className="pageOrTabInnerContainer">
            <div className="topLine">{topLine}</div>
            <div className="mainContent">{mainContent}</div>
            <div className="footer">{footer}</div>
            <div className="rightSidebar">{rightSidebar}</div>
        </div>
    )
}
