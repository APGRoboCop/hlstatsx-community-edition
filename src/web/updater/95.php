<?php
if ( !defined('IN_UPDATER') )
{
    die('Do not access this file directly.');
}

$dbversion = 95;
$version = "1.12.3";

echo "Executing update script 95 (Recompiled for CS2 AG2 (AnimGraph 2) engine update)...<br />";

// Update system version
$db->query("UPDATE hlstats_Options SET `value` = '$version' WHERE `keyname` = 'version'");
$db->query("UPDATE hlstats_Options SET `value` = '$dbversion' WHERE `keyname` = 'dbversion'");

echo "<br /><b>Update 95 Technical Summary:</b><br />";
echo "- <b>Core:</b> Recompiled for CS2 AG2 (AnimGraph 2) engine update to fix memory offsets and schema reflections.<br />";
echo "- <b>Modern Interface:</b> Integrated Async Database connectivity and CenterHtmlMenu support for enhanced in-game interaction.<br />";
echo "- <b>Accuracy:</b> Restored precise Hitgroup tracking (headshot, chest, etc.) by updating internal pointers to the latest Valve schema.<br />";
echo "<br />Update completed successfully.<br />";
?>