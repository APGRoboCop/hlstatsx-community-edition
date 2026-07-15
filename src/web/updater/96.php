<?php
if ( !defined('IN_UPDATER') )
{
    die('Do not access this file directly.');
}

$dbversion = 96;
$version = "1.12.4";

echo "Executing update script 96 (HLStatsX CS2 modernization CounterStrikeSharp v1.0.371 and .NET 10 compatibility update)...<br />";

// Update system version
$db->query("UPDATE hlstats_Options SET `value` = '$version' WHERE `keyname` = 'version'");
$db->query("UPDATE hlstats_Options SET `value` = '$dbversion' WHERE `keyname` = 'dbversion'");

echo "<br /><b>Update 96 Technical Summary:</b><br />";
echo "- <b>Backend:</b> Updated HLStatsX CS2 components for CounterStrikeSharp v1.0.371 and .NET 10 runtime and newer C# language compatibility.<br />";
echo "- <b>SuperLogs:</b> Improved weapon tracking, active weapon detection, death statistics accuracy and UDP communication reliability.<br />";
echo "- <b>Warmup Control:</b> Improved warmup detection handling with hot reload support and more reliable log state synchronization.<br />";
echo "- <b>Statistics:</b> Enhanced player weapon statistics processing with improved kill, death, hitgroup and damage tracking.<br />";
echo "- <b>Web Interface:</b> Updated modern-responsive.css to version 1.2 with improved dark mode chart support, mobile layout refinements and accessibility improvements.<br />";
echo "- <b>Language:</b> Added new profile editing timeout message support in HLStatsX CS2 translations.<br />";
echo "- <b>Database:</b> SQL schema unchanged. This update only updates the HLStatsX version information and does not modify database structure.<br />";
echo "<br />Update completed successfully.<br />";
?>