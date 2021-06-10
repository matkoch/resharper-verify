package com.jetbrains.rider.plugins.verify

import com.jetbrains.rider.unitTesting.actions.base.RiderUnitTestAnActionBase
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTarget
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetExecutor
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetOperation
import com.jetbrains.rider.unitTesting.actions.targets.RiderUnitTestTargetScope

class VerifyAcceptAction :
    RiderUnitTestAnActionBase("UnitTestSession.VerifyAccept") {
    override val target
        get() = RiderUnitTestTarget(
            RiderUnitTestTargetOperation.ElementManipulation,
            RiderUnitTestTargetExecutor.None,
            RiderUnitTestTargetScope.SelectedElements
        )

}

