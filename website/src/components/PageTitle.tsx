import { useEffect } from "react";
import { DEFAULT_TITLE } from "../constants";
import { Helmet, HelmetProvider } from "react-helmet-async";

function PageTitle(props: { title: string }) {

    useEffect(() => {
        document.title = props.title;

        return () => {
            document.title = DEFAULT_TITLE;
        }
    });

    return (
        <HelmetProvider>
            <Helmet prioritizeSeoTags>
                <meta property="og:title" content={props.title} />
                <title>{props.title}</title>
            </Helmet>
        </HelmetProvider>
    );
}

export default PageTitle;