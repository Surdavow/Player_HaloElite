%pattern = "add-ons/Player_HaloElite/sound/*.wav";
%file = findFirstFile(%pattern);
while(%file !$= "")
{
	%soundName = strreplace(filename(strlwr(%file)), ".wav", "");
	eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = AudioClose3d; filename = \"" @ %file @ "\"; };");
	%file = findNextFile(%pattern);
}

package DinoWort
{
	function ServerCmdPlantBrick(%client)
	{
		Parent::ServerCmdPlantBrick(%client);
		if(isObject(%player = %client.player) && %player.getDataBlock().getName() $= "HaloElitePlayer")
		{
			if(getSimTime()-%player.lastDinoLaughTime < 75 || (isObject(%player.getmountedImage(0)) && %player.getmountedImage(0).getName() $= "BrickImage")) return;								
			%player.stopaudio(0);
			%player.playaudio(0,"elite_wort" @ getRandom(1,5) @ "_sound");
			%player.lastDinoLaughTime = getSimTime();
		}
	}
};
activatePackage(DinoWort);

datablock TSShapeConstructor(HaloEliteDTS)
{
	baseShape  = "./models/elite.dts";
	sequence0  = "./models/elite.dsq";
};

datablock PlayerData(HaloElitePlayer : PlayerStandardArmor)
{
	shapeFile = HaloEliteDTS.baseShape;
	uiName = "Elite Player";

	cameramaxdist = 2.5;
	cameraVerticalOffset = 1.25;
	cameraHorizontalOffset = 0.8;
	cameraTilt = 0.2;
	maxfreelookangle = 3;

	runForce = 100 * 45;
	jumpforce = 100*10.5;
	jumpDelay = 25;
	minimpactspeed = 15;
	speedDamageScale = 0.5;
	mass = 105;
	airControl = 0.05;	

	groundImpactMinSpeed = 5;
	groundImpactShakeFreq = "4.0 4.0 4.0";
	groundImpactShakeAmp = "1.0 1.0 1.0";
	groundImpactShakeDuration = 0.8;
	groundImpactShakeFalloff = 15;	

	canJet = false;
	rideable = false;
	canRide = true;
	renderfirstperson = true;
	useCustomPainEffects = true;
	PainSound			= "";
	DeathSound			= "";
	jumpSound = "jumpSound";

	boundingBox = VectorScale ("2.25 1.25 3.25", 4);
	crouchBoundingBox = VectorScale ("2.25 1.25 2.75", 4);
};

function HaloElitePlayer::onImpact(%this, %obj, %col, %vec, %force)
{
	Parent::onImpact(%this, %obj, %col, %vec, %force);
	if(%obj.getState() !$= "Dead" && getWord(%vec,2)) %obj.playthread(0,"land");
}

function HaloElitePlayer::OnDamage(%this,%obj,%delta)
{
	Parent::OnDamage(%this,%obj,%delta);
	if(%obj.getState() !$= "Dead") %obj.playaudio(0,"elite_pain" @ getRandom(1,6) @ "_sound");
}

function HaloElitePlayer::OnDisabled(%this,%obj)
{
	Parent::OnDisabled(%this,%obj);
	if(%obj.getState() $= "Dead") %obj.playaudio(0,"elite_death" @ getRandom(1,5) @ "_sound");
}